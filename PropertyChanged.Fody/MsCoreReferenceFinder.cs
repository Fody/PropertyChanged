﻿using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference ComponentModelPropertyChangedEventHandlerInvokeReference;
    public MethodReference ComponentModelPropertyChangedEventConstructorReference;
    public MethodReference ActionConstructorReference;
    public MethodReference ObjectConstructor;
    public TypeReference ActionTypeReference;
    public MethodDefinition NullableEqualsMethod;
    public TypeReference PropChangedInterfaceReference;
    public TypeReference PropChangedHandlerReference;
    public TypeReference VoidTypeReference;
    public MethodReference DelegateCombineMethodRef;
    public MethodReference DelegateRemoveMethodRef;
    public MethodReference InterlockedCompareExchangeForPropChangedHandler;


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
        ObjectConstructor = ModuleDefinition.Import(constructorDefinition);


        var nullableDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

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
            actionDefinition = systemCoreDefinition.MainModule.Types.FirstOrDefault(x => x.Name == "Action");
        }
        ActionTypeReference = ModuleDefinition.Import(actionDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.Import(actionConstructor);

        var propChangedInterfaceDefinition = systemTypes.First(x => x.Name == "INotifyPropertyChanged");
        PropChangedInterfaceReference = ModuleDefinition.Import(propChangedInterfaceDefinition);
        var propChangedHandlerDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.Import(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = ModuleDefinition.Import(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = ModuleDefinition.Import(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        TypeDefinition delegateDefinition = msCoreTypes.First(x => x.Name == "Delegate");
        MethodDefinition combineMethodDefinition = delegateDefinition
            .Methods
            .Where(x => x.Name == "Combine")
            .Where(x => x.Parameters.Count == 2)
            .Where(x => x.Parameters.All(p => p.ParameterType == delegateDefinition))
            .Single();
        this.DelegateCombineMethodRef = ModuleDefinition.Import(combineMethodDefinition);
        MethodDefinition removeMethodDefinition = delegateDefinition.Methods.First(x => x.Name == "Remove");
        this.DelegateRemoveMethodRef = ModuleDefinition.Import(removeMethodDefinition);

        TypeDefinition voidDefinition = msCoreTypes.First(x => x.Name == "Void");
        this.VoidTypeReference = ModuleDefinition.Import(voidDefinition);

        TypeDefinition interlockedDefinition = msCoreTypes.First(x => x.FullName == "System.Threading.Interlocked");
        MethodDefinition genericCompareExchangeMethodDefinition = interlockedDefinition
            .Methods
            .Where(x => x.IsStatic)
            .Where(x => x.Name == "CompareExchange")
            .Where(x => x.GenericParameters.Count == 1)
            .Where(x => x.Parameters.Count == 3)
            .First();
        MethodReference genericCompareExchangeMethod = ModuleDefinition.Import(genericCompareExchangeMethodDefinition);

        var concreteCompareExchangeMethod = new GenericInstanceMethod(genericCompareExchangeMethod);
        concreteCompareExchangeMethod.GenericArguments.Add(this.PropChangedHandlerReference);
        InterlockedCompareExchangeForPropChangedHandler = concreteCompareExchangeMethod;
    }

    public void ExecuteWinRT()
    {
        var assemblyResolver = ModuleDefinition.AssemblyResolver;
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;

        var objectDefinition = systemRuntimeTypes.First(x => x.Name == "Object");

        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = ModuleDefinition.Import(constructorDefinition);


        var nullableDefinition = systemRuntimeTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = ModuleDefinition.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");


        var actionDefinition = systemRuntimeTypes.First(x => x.Name == "Action");
        ActionTypeReference = ModuleDefinition.Import(actionDefinition);
        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = ModuleDefinition.Import(actionConstructor);


        var systemObjectModel = assemblyResolver.Resolve("System.ObjectModel");
        var systemObjectModelTypes = systemObjectModel.MainModule.Types;
        var propChangedHandlerDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = ModuleDefinition.Import(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = ModuleDefinition.Import(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = ModuleDefinition.Import(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var windowsRuntime = assemblyResolver.Resolve("System.Runtime.InteropServices.WindowsRuntime");
        var genericInstanceType = new GenericInstanceType(windowsRuntime.MainModule.Types.First(x => x.Name == "EventRegistrationTokenTable`1"));
        genericInstanceType.GenericArguments.Add(PropChangedHandlerReference);

    }


    AssemblyDefinition GetSystemCoreDefinition()
    {
        try
        {
            return ModuleDefinition.AssemblyResolver.Resolve("System.Core");
        }
        catch (Exception exception)
        {
            var message = string.Format(@"Could not resolve System.Core. Please ensure you are using .net 3.5 or higher.{0}Inner message:{1}.", Environment.NewLine, exception.Message);
            throw new WeavingException(message);
        }
    }
}