using System.Collections.Generic;
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
            notifyNode.ExplicitOnChangedMethods = GetExplicitOnChangedMethods(notifyNode).ToList();
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
                var message = string.Format("The type {0} has a On_PropertyName_Changed method ({1}) that has a non void return value. Please make the return type void.", notifyNode.TypeDefinition.FullName, methodDefinition.Name);
                throw new WeavingException(message);
            }
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

    IEnumerable<ExplicitOnChangedMethod> GetExplicitOnChangedMethods(TypeNode notifyNode)
    {
        var methods = notifyNode.TypeDefinition.Methods;

        var onChangedMethods = methods.Where(x => !x.IsStatic &&
                                  x.CustomAttributes.GetAttribute("PropertyChanged.OnPropertyChangedAttribute") != null);

        foreach (var methodDefinition in onChangedMethods)
        {
            if (methodDefinition.ReturnType.FullName != "System.Void")
            {
                var message = string.Format("The type {0} has a On_PropertyName_Changed method ({1}) that has a non void return value. Please make the return type void.", notifyNode.TypeDefinition.FullName, methodDefinition.Name);
                throw new WeavingException(message);
            }
            var typeDefinitions = new Stack<TypeDefinition>();
            typeDefinitions.Push(notifyNode.TypeDefinition);

            if (IsNoArgOnChangedMethod(methodDefinition))
            {
                yield return new ExplicitOnChangedMethod
                {
                    OnChangedType = OnChangedTypes.NoArg,
                    MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                    CustomAttributes = methodDefinition.CustomAttributes
                };
            }
            else if (IsBeforeAfterOnChangedMethod(methodDefinition))
            {
                yield return new ExplicitOnChangedMethod
                {
                    OnChangedType = OnChangedTypes.BeforeAfter,
                    MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                    CustomAttributes = methodDefinition.CustomAttributes
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