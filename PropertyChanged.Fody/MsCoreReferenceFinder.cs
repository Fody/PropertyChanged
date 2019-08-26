using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    public MethodReference PropertyChangedEventHandlerInvokeReference;
    public TypeReference PropertyChangedEventArgsReference;
    public MethodReference PropertyChangedEventConstructorReference;
    public MethodReference ActionConstructorReference;
    public MethodReference ObjectConstructor;
    public MethodReference ObjectEqualsMethod;
    public TypeReference EqualityComparerTypeReference;
    public TypeReference ActionTypeReference;
    public MethodDefinition NullableEqualsMethod;
    public TypeReference PropChangedInterfaceReference;
    public TypeReference PropChangedHandlerReference;
    public MethodReference DelegateCombineMethodRef;
    public MethodReference DelegateRemoveMethodRef;
    public GenericInstanceMethod InterlockedCompareExchangeForPropChangedHandler;
    public Lazy<MethodReference> Trigger;
    public MethodReference StringEquals;
    public MethodReference DebuggerNonUserCodeAttributeConstructor;
    public MethodReference GeneratedCodeAttributeConstructor;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "System.Core";
        yield return "netstandard";
        yield return "System.Collections";
        yield return "System.ObjectModel";
        yield return "System.Threading";
        yield return "FSharp.Core";
        //TODO: remove when move to only netstandard2.0
        yield return "System.Diagnostics.Tools";
        yield return "System.Diagnostics.Debug";
    }

    public void FindCoreReferences()
    {
        var objectDefinition = FindType("System.Object");
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);
        var objectEqualsMethodDefinition = objectDefinition.Methods.First(x => x.Name == "Equals" && x.Parameters.Count == 2);
        ObjectEqualsMethod = ModuleDefinition.ImportReference(objectEqualsMethodDefinition);

        var stringEquals = FindType("System.String")
            .Methods
            .First(x => x.IsStatic &&
                        x.Name == "Equals" &&
                        x.Parameters.Count == 3 &&
                        x.Parameters[0].ParameterType.Name == "String" &&
                        x.Parameters[1].ParameterType.Name == "String" &&
                        x.Parameters[2].ParameterType.Name == "StringComparison");
        StringEquals = ModuleDefinition.ImportReference(stringEquals);

        var nullableDefinition = FindType("System.Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        EqualityComparerTypeReference = FindType("System.Collections.Generic.EqualityComparer`1");

        var actionDefinition = FindType("System.Action");
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var propChangedInterfaceDefinition = FindType("System.ComponentModel.INotifyPropertyChanged");
        PropChangedInterfaceReference = ModuleDefinition.ImportReference(propChangedInterfaceDefinition);
        var propChangedHandlerDefinition = FindType("System.ComponentModel.PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition);
        PropertyChangedEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = FindType("System.ComponentModel.PropertyChangedEventArgs");
        PropertyChangedEventArgsReference = ModuleDefinition.ImportReference(propChangedArgsDefinition);
        PropertyChangedEventConstructorReference = ModuleDefinition.ImportReference(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = FindType("System.Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var interlockedDefinition = FindType("System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x =>
                x.IsStatic &&
                x.Name == "CompareExchange" &&
                x.GenericParameters.Count == 1 &&
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.ImportReference(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangedHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangedHandler.GenericArguments.Add(PropChangedHandlerReference);
        Trigger = new Lazy<MethodReference>(() =>
        {
            if (TryFindType("Microsoft.FSharp.Control.FSharpEvent`2", out var fSharpEvent))
            {
                var trigger = fSharpEvent.Methods.Single(x => x.Name == "Trigger");
                return ModuleDefinition.ImportReference(trigger.MakeGeneric(PropChangedHandlerReference, propChangedArgsDefinition));
            }

            return null;
        });

        var generatedCodeType = FindType("System.CodeDom.Compiler.GeneratedCodeAttribute");
        var generatedCodeAttributeConstructor = generatedCodeType.GetConstructors().Single(c => c.Parameters.Count == 2 && c.Parameters.All(p => p.ParameterType.Name == "String"));
        GeneratedCodeAttributeConstructor = ModuleDefinition.ImportReference(generatedCodeAttributeConstructor);

        var debuggerNonUserCodeType = FindType("System.Diagnostics.DebuggerNonUserCodeAttribute");
        var debuggerNonUserCodeConstructor = debuggerNonUserCodeType.GetConstructors().Single(c => !c.HasParameters);
        DebuggerNonUserCodeAttributeConstructor = ModuleDefinition.ImportReference(debuggerNonUserCodeConstructor);
    }
}