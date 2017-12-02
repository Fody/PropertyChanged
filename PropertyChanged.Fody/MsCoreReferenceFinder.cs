using System.Collections.Generic;
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

    void AddAssemblyIfExists(string name, List<TypeDefinition> types)
    {
        try
        {
            var msCoreLibDefinition = AssemblyResolver.Resolve(new AssemblyNameReference(name, null));

            if (msCoreLibDefinition != null)
            {
                types.AddRange(msCoreLibDefinition.MainModule.Types);
            }
        }
        catch (AssemblyResolutionException)
        {
            LogInfo($"Failed to resolve '{name}'. So skipping its types.");
        }
    }

    public void FindCoreReferences()
    {
        var types = new List<TypeDefinition>();

        AddAssemblyIfExists("mscorlib", types);
        AddAssemblyIfExists("System", types);
        AddAssemblyIfExists("System.Runtime", types);
        AddAssemblyIfExists("System.Core", types);
        AddAssemblyIfExists("netstandard", types);
        AddAssemblyIfExists("System.ObjectModel", types);
        AddAssemblyIfExists("System.Threading", types);
        AddAssemblyIfExists("FSharp.Core", types);

        var objectDefinition = types.First(x => x.Name == "Object");
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.ImportReference(constructorDefinition);
        var objectEqualsMethodDefinition = objectDefinition.Methods.First(x => x.Name == "Equals" && x.Parameters.Count == 2);
        ObjectEqualsMethod = ModuleDefinition.ImportReference(objectEqualsMethodDefinition);

        var nullableDefinition = types.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.ImportReference(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        EqualityComparerTypeReference = types.FirstOrDefault(x => x.Name == "EqualityComparer`1");

        var actionDefinition = types.First(x => x.Name == "Action");
        ActionTypeReference = ModuleDefinition.ImportReference(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.ImportReference(actionConstructor);

        var propChangedInterfaceDefinition = types.First(x => x.Name == "INotifyPropertyChanged");
        PropChangedInterfaceReference = ModuleDefinition.ImportReference(propChangedInterfaceDefinition);
        var propChangedHandlerDefinition = types.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = ModuleDefinition.ImportReference(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = types.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = ModuleDefinition.ImportReference(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var delegateDefinition = types.First(x => x.Name == "Delegate");
        var combineMethodDefinition = delegateDefinition.Methods
            .Single(x =>
                x.Name == "Combine" &&
                x.Parameters.Count == 2 &&
                x.Parameters.All(p => p.ParameterType == delegateDefinition));
        DelegateCombineMethodRef = ModuleDefinition.ImportReference(combineMethodDefinition);
        var removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        DelegateRemoveMethodRef = ModuleDefinition.ImportReference(removeMethodDefinition);

        var interlockedDefinition = types.First(x => x.FullName == "System.Threading.Interlocked");
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
}