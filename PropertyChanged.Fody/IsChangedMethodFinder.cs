﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    string isChangedPropertyName = "IsChanged";

    void ProcessChildNode(TypeNode node, MethodReference changedInvokerMethod)
    {
        var childEventInvoker = FindEventInvokerMethodRef(node.TypeDefinition);
        if (childEventInvoker == null)
        {
            if (changedInvokerMethod != null)
            {
                if (node.TypeDefinition.BaseType.IsGenericInstance)
                {
                    var methodReference = MakeGeneric(node.TypeDefinition.BaseType, changedInvokerMethod);
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
            if (FindIsChangedEventInvokerMethodDefinition(currentTypeDefinition, out methodDefinition))
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

        return GetMethodReference(typeDefinitions, methodDefinition);
    }


    MethodReference FindEventInvokerMethodRef(TypeDefinition type)
    {
        MethodDefinition methodDefinition;
        if (FindIsChangedEventInvokerMethodDefinition(type, out methodDefinition))
        {
            var methodReference = ModuleDefinition.Import(methodDefinition);
            return methodReference.GetGeneric();
        }
        return null;
    }

    bool FindIsChangedEventInvokerMethodDefinition(TypeDefinition type, out MethodDefinition methodDefinition)
    {
        //todo: check bool type
        methodDefinition = null;
        var propertyDefinition = type.Properties
            .FirstOrDefault(x =>
                            x.Name == isChangedPropertyName &&
                            x.SetMethod != null &&
                            x.SetMethod.IsPublic
            );


        if (propertyDefinition != null)
        {
            if (propertyDefinition.PropertyType.FullName != ModuleDefinition.TypeSystem.Boolean.FullName)
            {
                LogWarning(string.Format("Found '{0}' but is was of type '{1}' instead of '{2}' so it will not be used.", propertyDefinition.GetName(), propertyDefinition.PropertyType.Name, ModuleDefinition.TypeSystem.Boolean.Name));
                return false;
            }
            methodDefinition = propertyDefinition.SetMethod;
        }
        return methodDefinition != null;
    }

    public void FindIsChangedMethod()
    {
        foreach (var notifyNode in NotifyNodes)
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