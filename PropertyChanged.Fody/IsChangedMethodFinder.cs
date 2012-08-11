using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class IsChangedMethodFinder
{
    MethodGenerifier methodGenerifier;
    ModuleWeaver moduleWeaver;
    TypeNodeBuilder typeNodeBuilder;
    TypeResolver typeResolver;
    TypeSystem typeSystem;
    string isChangedPropertyName = "IsChanged";

    public IsChangedMethodFinder(MethodGenerifier methodGenerifier, ModuleWeaver moduleWeaver, TypeNodeBuilder typeNodeBuilder, TypeResolver typeResolver, TypeSystem typeSystem)
    {
        this.methodGenerifier = methodGenerifier;
        this.moduleWeaver = moduleWeaver;
        this.typeNodeBuilder = typeNodeBuilder;
        this.typeResolver = typeResolver;
        this.typeSystem = typeSystem;
    }


    void ProcessChildNode(TypeNode node, MethodReference changedInvokerMethod)
    {
        var childEventInvoker = FindEventInvokerMethod(node.TypeDefinition);
        if (childEventInvoker == null)
        {
            if (changedInvokerMethod != null)
            {
                if (node.TypeDefinition.BaseType.IsGenericInstance)
                {
                    var methodReference = MethodGenerifier.MakeGeneric(node.TypeDefinition.BaseType, changedInvokerMethod);
                    changedInvokerMethod = methodReference;
                }
            }
        }
        else
        {
            changedInvokerMethod = childEventInvoker;
        }

        node.IsChangedInvoker = changedInvokerMethod;

        foreach (var childNode in node.Nodes)
        {
            ProcessChildNode(childNode, changedInvokerMethod);
        }
    }



    public MethodReference RecursiveFindMethod(TypeDefinition typeDefinition)
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
            currentTypeDefinition = typeResolver.Resolve(baseType);
        } while (true);

        return methodGenerifier.GetMethodReference(typeDefinitions, methodDefinition);
    }


    MethodReference FindEventInvokerMethod(TypeDefinition type)
    {
        MethodDefinition methodDefinition;
        if (FindEventInvokerMethodDefinition(type, out methodDefinition))
        {
            var methodReference = moduleWeaver.ModuleDefinition.Import(methodDefinition);
            return methodReference.GetGeneric();
        }
        return null;
    }

    bool FindEventInvokerMethodDefinition(TypeDefinition type, out MethodDefinition methodDefinition)
    {
        //todo: check bool type
        methodDefinition = null;
        var propertyDefinition = type.Properties
            .FirstOrDefault(x =>
                            x.Name == isChangedPropertyName &&
                            x.SetMethod != null
            );


        if (propertyDefinition != null)
        {
            if (propertyDefinition.PropertyType.FullName != typeSystem.Boolean.FullName)
            {
                moduleWeaver.LogWarning(string.Format("Found '{0}' but is was of type '{1}' instead of '{2}' so it will not be used.", propertyDefinition.GetName(), propertyDefinition.PropertyType.Name, typeSystem.Boolean.Name));
                return false;
            }
            methodDefinition = propertyDefinition.SetMethod;
        }
        return methodDefinition != null;
    }

    public void Execute()
    {
        foreach (var notifyNode in typeNodeBuilder.NotifyNodes)
        {
            var eventInvoker = RecursiveFindMethod(notifyNode.TypeDefinition);

            notifyNode.IsChangedInvoker = eventInvoker;

            foreach (var childNode in notifyNode.Nodes)
            {
                ProcessChildNode(childNode, eventInvoker);
            }
        }
    }
}