﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void ProcessOnChangedMethods()
    {
        ProcessOnChangedMethods(NotifyNodes);
    }

    void ProcessOnChangedMethods(List<TypeNode> notifyNodes)
    {
        foreach (var notifyNode in notifyNodes)
        {
            notifyNode.OnChangedMethods = GetOnChangedMethods(notifyNode).ToList();
            ProcessOnChangedMethods(notifyNode.Nodes);
        }
    }

    IEnumerable<OnChangedMethod> GetOnChangedMethods(TypeNode notifyNode)
    {
        var methods = notifyNode.TypeDefinition.Methods;

        var onChangedMethods = methods.Where(x => !x.IsStatic &&
                                  !x.IsAbstract &&
                                  //(IsNoArgOnChangedMethod(x) ||
                                  //IsBeforeAfterOnChangedMethod(x)) &&
                                  x.Name.StartsWith("On") &&
                                  x.Name.EndsWith("Changed"));

        foreach (var methodDefinition in onChangedMethods)
        {
            var typeDefinitions = new Stack<TypeDefinition>();
            typeDefinitions.Push(notifyNode.TypeDefinition);

            if (IsNoArgOnChangedMethod(methodDefinition))
            {
                yield return new OnChangedMethod
                {
                    OnChangedType = OnChangedTypes.NoArg,
                    MethodReference = GetMethodReference(typeDefinitions, methodDefinition)
                };
            }
            else if (IsBeforeAfterOnChangedMethod(methodDefinition))
            {
                yield return new OnChangedMethod
                {
                    OnChangedType = OnChangedTypes.BeforeAfter,
                    MethodReference = GetMethodReference(typeDefinitions, methodDefinition)
                };
            }            
        }
    }

    public static bool IsNoArgOnChangedMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 0;
    }

    public static bool IsBeforeAfterOnChangedMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 2
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Object";
    }
}