using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ComponentModelPropertyChangedEventHandlerInvokeReference;
    public MethodReference ComponentModelPropertyChangedEventConstructorReference;
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

    public void FindCoreReferences()
    {
        var assemblyResolver = ModuleDefinition.AssemblyResolver;
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            ExecuteWinRT();
            return;
        }
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);
        var objectEqualsMethodDefinition = objectDefinition.Methods.First(x => x.Name == "Equals" && x.Parameters.Count == 2);
        ObjectEqualsMethod = ModuleDefinition.ImportReference(objectEqualsMethodDefinition);

        var nullableDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        EqualityComparerTypeReference = msCoreTypes.FirstOrDefault(x => x.Name == "EqualityComparer`1");

        var systemDefinition = assemblyResolver.Resolve("System");
        var systemTypes = systemDefinition.MainModule.Types;

        var actionDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Action");
        if (actionDefinition == null)
        {
            actionDefinition = systemTypes.FirstOrDefault(x => x.Name == "Action");
        }
        var systemCoreDefinition = GetSystemCoreDefinition();
        if (actionDefinition == null)
        {
            actionDefinition = systemCoreDefinition.MainModule.Types.First(x => x.Name == "Action");
        }
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var propChangedInterfaceDefinition = systemTypes.First(x => x.Name == "INotifyPropertyChanged");
        PropChangedInterfaceReference = ModuleDefinition.ImportReference(propChangedInterfaceDefinition);
        var propChangedHandlerDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = ModuleDefinition.ImportReference(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = msCoreTypes.First(x => x.Name == "Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var interlockedDefinition = msCoreTypes.First(x => x.FullName == "System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x =>
                x.IsStatic &&
                x.Name == "CompareExchange" &&
                x.GenericParameters.Count == 1 &&
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.ImportReference(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangedHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangedHandler.GenericArguments.Add(PropChangedHandlerReference);
    }

    public void ExecuteWinRT()
    {
        var assemblyResolver = ModuleDefinition.AssemblyResolver;
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;

        var objectDefinition = systemRuntimeTypes.First(x => x.Name == "Object");

        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);
        var objectEqualsMethodDefinition = objectDefinition.Methods.First(x => x.Name == "Equals" && x.Parameters.Count == 2);
        ObjectEqualsMethod = ModuleDefinition.ImportReference(objectEqualsMethodDefinition);

        var nullableDefinition = systemRuntimeTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        EqualityComparerTypeReference = systemRuntimeTypes.FirstOrDefault(x => x.Name == "EqualityComparer`1");

        var actionDefinition = systemRuntimeTypes.First(x => x.Name == "Action");
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);
        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var systemObjectModel = assemblyResolver.Resolve("System.ObjectModel");
        var systemObjectModelTypes = systemObjectModel.MainModule.Types;

        var propChangedInterfaceDefinition = systemObjectModelTypes.First(x => x.Name == "INotifyPropertyChanged");
        PropChangedInterfaceReference = ModuleDefinition.ImportReference(propChangedInterfaceDefinition);

        var propChangedHandlerDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = ModuleDefinition.ImportReference(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = systemRuntimeTypes.First(x => x.Name == "Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var systemThreading = assemblyResolver.Resolve("System.Threading");
        var interlockedDefinition = systemThreading.MainModule.Types.First(x => x.FullName == "System.Threading.Interlocked");
        var genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods.First(x =>
                x.IsStatic &&
                x.Name == "CompareExchange" &&
                x.GenericParameters.Count == 1 &&
                x.Parameters.Count == 3);
        var genericCompareExchangeMethod = ModuleDefinition.ImportReference(genericCompareExchangeMethodDefinition);

        InterlockedCompareExchangeForPropChangedHandler = new GenericInstanceMethod(genericCompareExchangeMethod);
        InterlockedCompareExchangeForPropChangedHandler.GenericArguments.Add(PropChangedHandlerReference);
    }

    AssemblyDefinition GetSystemCoreDefinition()
    {
        try
        {
            return ModuleDefinition.AssemblyResolver.Resolve("System.Core");
        }
        catch (Exception exception)
        {
            var message = $@"Could not resolve System.Core. Please ensure you are using .net 3.5 or higher.{Environment.NewLine}Inner message:{exception.Message}.";
            throw new WeavingException(message);
        }
    }
}