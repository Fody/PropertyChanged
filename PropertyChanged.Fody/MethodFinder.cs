using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public partial class ModuleWeaver
{
    void ProcessChildNode(TypeNode node, EventInvokerMethod eventInvoker)
    {
        var childEventInvoker = FindEventInvokerMethod(node.TypeDefinition);
        if (childEventInvoker == null)
        {
            if (eventInvoker.MethodReference.DeclaringType.ContainsGenericParameter)
            {
                var methodReference = MakeGeneric(node.TypeDefinition.BaseType, eventInvoker.MethodReference);
                eventInvoker = new()
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
            var error = $"Cannot use '{eventInvoker.MethodReference.FullName}' in '{node.TypeDefinition.FullName}' since that method is not visible from the child class.";
            throw new WeavingException(error);
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

            methodDefinition = FindEventInvokerMethodDefinition(currentTypeDefinition);
            if (methodDefinition != null)
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

        return new()
        {
            MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
            IsVisibleFromChildren = IsVisibleFromChildren(methodDefinition),
            InvokerType = ClassifyInvokerMethod(methodDefinition),
        };
    }

    static bool IsVisibleFromChildren(MethodDefinition method)
    {
        return method.IsFamilyOrAssembly ||
               method.IsFamily ||
               method.IsFamilyAndAssembly ||
               method.IsPublic;
    }

    EventInvokerMethod FindEventInvokerMethod(TypeDefinition type)
    {
        var methodDefinition = FindEventInvokerMethodDefinition(type);

        if (methodDefinition == null || methodDefinition.IsPrivate)
        {
            var overriddenMethod = FindExplicitImplementation(type);
            if (overriddenMethod != null)
            {
                return new()
                {
                    MethodReference = ModuleDefinition.ImportReference(overriddenMethod).GetGeneric(),
                    IsVisibleFromChildren = true,
                    InvokerType = ClassifyInvokerMethod(overriddenMethod)
                };
            }
        }

        if (methodDefinition == null)
        {
            return null;
        }

        var methodReference = ModuleDefinition.ImportReference(methodDefinition);
        return new()
        {
            MethodReference = methodReference.GetGeneric(),
            IsVisibleFromChildren = IsVisibleFromChildren(methodDefinition),
            InvokerType = ClassifyInvokerMethod(methodDefinition),
        };
    }

    MethodDefinition FindEventInvokerMethodDefinition(TypeDefinition type)
    {
        var methodDefinition = type.Methods
            .Where(x => (x.IsFamily || x.IsFamilyAndAssembly || x.IsPublic || x.IsFamilyOrAssembly) &&
                        EventInvokerNames.Contains(x.Name))
            .OrderByDescending(GetInvokerPriority)
            .FirstOrDefault(IsEventInvokerMethod);

        if (methodDefinition != null)
        {
            return methodDefinition;
        }

        methodDefinition = type.Methods
            .Where(x => EventInvokerNames.Contains(x.Name))
            .OrderByDescending(GetInvokerPriority)
            .FirstOrDefault(IsEventInvokerMethod);

        if (methodDefinition is {IsPrivate: true, IsFinal: true, IsVirtual: true} &&
            methodDefinition.Overrides.Count == 1)
        {
            // Explicitly implemented interfaces should call the interface method instead
            return methodDefinition.Overrides[0].Resolve();
        }

        return methodDefinition;
    }

    MethodReference FindExplicitImplementation(TypeDefinition type)
    {
        return type.GetAllInterfaces()
            .Select(i => i.Resolve())
            .Where(i => i is {IsPublic: true})
            .SelectMany(i => i.Methods.Where(m => EventInvokerNames.Contains($"{i.FullName}.{m.Name}")))
            .OrderByDescending(GetInvokerPriority)
            .FirstOrDefault(IsEventInvokerMethod);
    }

    static int GetInvokerPriority(MethodReference method)
    {
        if (IsBeforeAfterGenericMethod(method))
        {
            return 5;
        }

        if (IsBeforeAfterMethod(method))
        {
            return 4;
        }

        if (IsSenderPropertyChangedArgMethod(method))
        {
            return 3;
        }

        if (IsPropertyChangedArgMethod(method))
        {
            return 2;
        }

        if (IsSingleStringMethod(method))
        {
            return 1;
        }

        return 0;
    }

    static InvokerTypes ClassifyInvokerMethod(MethodReference method)
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

    static bool IsEventInvokerMethod(MethodReference method)
    {
        return IsBeforeAfterGenericMethod(method)
               || IsBeforeAfterMethod(method)
               || IsSingleStringMethod(method)
               || IsPropertyChangedArgMethod(method)
               || IsSenderPropertyChangedArgMethod(method);
    }

    public static bool IsSenderPropertyChangedArgMethod(MethodReference method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 2
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.ComponentModel.PropertyChangedEventArgs";
    }

    public static bool IsPropertyChangedArgMethod(MethodReference method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.ComponentModel.PropertyChangedEventArgs";
    }

    public static bool IsSingleStringMethod(MethodReference method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 1
               && parameters[0].ParameterType.FullName == "System.String";
    }

    public static bool IsBeforeAfterMethod(MethodReference method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && parameters[0].ParameterType.FullName == "System.String"
               && parameters[1].ParameterType.FullName == "System.Object"
               && parameters[2].ParameterType.FullName == "System.Object";
    }

    public static bool IsBeforeAfterGenericMethod(MethodReference method)
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
