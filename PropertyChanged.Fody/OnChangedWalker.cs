using System.Collections.Generic;
using System.Linq;
using Fody;
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
                                  x.Name.StartsWith("On") &&
                                  x.Name.EndsWith("Changed"));

        foreach (var methodDefinition in onChangedMethods)
        {
            if (methodDefinition.ReturnType.FullName != "System.Void")
            {
                var message = $"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) that has a non void return value. Ensure the return type void.";
                throw new WeavingException(message);
            }
            var typeDefinitions = new Stack<TypeDefinition>();
            typeDefinitions.Push(notifyNode.TypeDefinition);

            var onChangedType = GetOnChangedType(methodDefinition);
            if (onChangedType != null)
            {
                yield return new OnChangedMethod
                {
                    OnChangedType = onChangedType.Value,
                    MethodReference = GetMethodReference(typeDefinitions, methodDefinition)
                };
            }
        }
    }

    OnChangedTypes? GetOnChangedType(IMethodSignature methodDefinition)
    {
        var parameters = methodDefinition.Parameters;

        if (parameters.Count == 0)
            return OnChangedTypes.NoArg;

        if (parameters.Count == 2 && parameters[0].ParameterType == parameters[1].ParameterType)
        {
            var parameterType = parameters[0].ParameterType;
            if (parameterType.FullName == "System.Object")
                return OnChangedTypes.BeforeAfterObject;

            return OnChangedTypes.BeforeAfterType;
        }

        return null;
    }
}