using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    void ProcessChildNode(TypeNode node, EventInvokerMethod eventInvoker)
    {
        var childEventInvoker = FindEventInvokerMethod(node.TypeDefinition);
        if (childEventInvoker == null)
        {
            if (node.TypeDefinition.BaseType.IsGenericInstance)
            {
                var methodReference = MakeGeneric(node.TypeDefinition.BaseType, eventInvoker.MethodReference);
                eventInvoker = new EventInvokerMethod
                                   {
                                       InvokerType = eventInvoker.InvokerType,
                                       MethodReference = methodReference,
                                       IsVisibleFromChildren = eventInvoker.IsVisibleFromChildren
                                   };
            }
        }
        else
        {
            eventInvoker = childEventInvoker;
        }

        if (!eventInvoker.IsVisibleFromChildren)
        {
            var methodName = eventInvoker.MethodReference.Name;
            var viewModelBaseType = node.TypeDefinition.BaseType.Resolve();
            
            // WARN: Not finished yet.
            // TODO: Support nested hierarchies.
            foreach (var implementation in viewModelBaseType.Interfaces)
            {
                var interfaceType = implementation.InterfaceType;
                var interfaceResolution = interfaceType.Resolve();
                var method = interfaceResolution.Methods.FirstOrDefault(x => methodName.Contains(x.Name));
                if (method == null) continue;

                var methodReference = MakeGeneric(interfaceType, method);
                eventInvoker = new EventInvokerMethod
                {
                    IsVisibleFromChildren = eventInvoker.IsVisibleFromChildren,
                    InvokerType = eventInvoker.InvokerType,
                    MethodReference = methodReference
                };
            }
        }
        
        node.EventInvoker = eventInvoker;
        foreach (var childNode in node.Nodes)
        {
            ProcessChildNode(childNode, eventInvoker);
        }
    }

    public EventInvokerMethod RecursiveFindEventInvoker(TypeDefinition typeDefinition)
    {
        var typeDefinitions = new Stack<TypeDefinition>();
        MethodDefinition methodDefinition;
        var currentTypeDefinition = typeDefinition;
        do
        {
            typeDefinitions.Push(currentTypeDefinition);

            if (FindEventInvokerMethodDefinition(currentTypeDefinition, out methodDefinition))
            {
                break;
            }
            var baseType = currentTypeDefinition.BaseType;

            if (baseType == null || baseType.FullName == "System.Object")
            {
                return null;
            }
            currentTypeDefinition = Resolve(baseType);
        } while (true);

        return new EventInvokerMethod
                   {
                       MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                       IsVisibleFromChildren = IsVisibleFromChildren(methodDefinition),
                       InvokerType = ClassifyInvokerMethod(methodDefinition),
                   };
    }

    static bool IsVisibleFromChildren(MethodDefinition methodDefinition)
    {
        return methodDefinition.IsFamilyOrAssembly || methodDefinition.IsFamily || methodDefinition.IsFamilyAndAssembly || methodDefinition.IsPublic;
    }

    EventInvokerMethod FindEventInvokerMethod(TypeDefinition type)
    {
        if (!FindEventInvokerMethodDefinition(type, out var methodDefinition))
        {
            return null;
        }
        var methodReference = ModuleDefinition.ImportReference(methodDefinition);
        return new EventInvokerMethod
        {
            MethodReference = methodReference.GetGeneric(),
            IsVisibleFromChildren = IsVisibleFromChildren(methodDefinition),
            InvokerType = ClassifyInvokerMethod(methodDefinition),
        };
    }

    bool FindEventInvokerMethodDefinition(TypeDefinition type, out MethodDefinition methodDefinition)
    {
        methodDefinition = type.Methods
            .Where(x => (x.IsFamily || x.IsFamilyAndAssembly || x.IsPublic || x.IsFamilyOrAssembly) && EventInvokerNames.Contains(x.Name))
            .OrderByDescending(GetInvokerPriority)
            .FirstOrDefault(x => IsBeforeAfterGenericMethod(x) || IsBeforeAfterMethod(x) || IsSingleStringMethod(x) || IsPropertyChangedArgMethod(x) || IsSenderPropertyChangedArgMethod(x));
        if (methodDefinition == null)
        {
            methodDefinition = type.Methods
                .Where(x => EventInvokerNames.Contains(x.Name))
                .OrderByDescending(GetInvokerPriority)
                .FirstOrDefault(x => IsBeforeAfterGenericMethod(x) || IsBeforeAfterMethod(x) || IsSingleStringMethod(x) || IsPropertyChangedArgMethod(x) || IsSenderPropertyChangedArgMethod(x));
        }
        return methodDefinition != null;
    }

    static int GetInvokerPriority(MethodDefinition method)
    {
        if (IsBeforeAfterGenericMethod(method))
            return 5;

        if (IsBeforeAfterMethod(method))
            return 4;

        if (IsSenderPropertyChangedArgMethod(method))
            return 3;

        if (IsPropertyChangedArgMethod(method))
            return 2;

        if (IsSingleStringMethod(method))
            return 1;

        return 0;
    }

    public static InvokerTypes ClassifyInvokerMethod(MethodDefinition method)
    {
        if (IsSenderPropertyChangedArgMethod(method))
        {
            return InvokerTypes.SenderPropertyChangedArg;
        }
        if (IsPropertyChangedArgMethod(method))
        {
            return InvokerTypes.PropertyChangedArg;
        }
        if (IsBeforeAfterMethod(method))
        {
            return InvokerTypes.BeforeAfter;
        }
        if (IsBeforeAfterGenericMethod(method))
        {
            return InvokerTypes.BeforeAfterGeneric;
        }

        return InvokerTypes.String;
    }

    public static bool IsSenderPropertyChangedArgMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 2
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.ComponentModel.PropertyChangedEventArgs";
    }

    public static bool IsPropertyChangedArgMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.ComponentModel.PropertyChangedEventArgs";
    }

    public static bool IsSingleStringMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.String";
    }

    public static bool IsBeforeAfterMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && parameters[0].ParameterType.FullName == "System.String"
               && parameters[1].ParameterType.FullName == "System.Object"
               && parameters[2].ParameterType.FullName == "System.Object";
    }

    public static bool IsBeforeAfterGenericMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && method.HasGenericParameters
               && method.GenericParameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.String"
               && parameters[1].ParameterType.FullName == method.GenericParameters[0].FullName
               && parameters[2].ParameterType.FullName == method.GenericParameters[0].FullName;
    }

    public void FindMethodsForNodes()
    {
        foreach (var notifyNode in NotifyNodes)
        {
            var eventInvoker = RecursiveFindEventInvoker(notifyNode.TypeDefinition);
            if (eventInvoker == null)
            {
                eventInvoker = AddOnPropertyChangedMethod(notifyNode.TypeDefinition);
            }

            notifyNode.EventInvoker = eventInvoker;

            foreach (var childNode in notifyNode.Nodes)
            {
                ProcessChildNode(childNode, eventInvoker);
            }
        }
    }
}