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
            ProcessOnChangedMethods(notifyNode);
            ProcessOnChangedMethods(notifyNode.Nodes);
        }
    }

    void ProcessOnChangedMethods(TypeNode notifyNode)
    {
        var methods = new Dictionary<string, OnChangedMethod>();

        if (InjectOnPropertyNameChanged)
        {
            foreach (var methodDefinition in notifyNode.TypeDefinition.Methods)
            {
                var methodName = methodDefinition.Name;

                if (!methodName.StartsWith("On") || !methodName.EndsWith("Changed") || methodName == "OnChanged")
                {
                    continue;
                }

                var onChangedMethod = CreateOnChangedMethod(notifyNode, methodDefinition, true);
                if (onChangedMethod == null)
                {
                    continue;
                }

                if (methods.ContainsKey(methodName))
                {
                    throw new WeavingException($"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) which has multiple valid overloads.");
                }

                methods.Add(methodName, onChangedMethod);
            }
        }

        foreach (var propertyData in notifyNode.PropertyDatas)
        {
            var hasCustomMethods = false;

            foreach (var attribute in propertyData.PropertyDefinition.CustomAttributes.GetAttributes("PropertyChanged.OnChangedMethodAttribute"))
            {
                hasCustomMethods = true;
                var methodName = (string)attribute.ConstructorArguments[0].Value;

                if (string.IsNullOrEmpty(methodName))
                {
                    continue;
                }

                if (!methods.TryGetValue(methodName, out var onChangedMethod))
                {
                    onChangedMethod = FindOnChangedMethod(notifyNode, methodName);
                    methods.Add(methodName, onChangedMethod);
                }

                propertyData.OnChangedMethods.Add(onChangedMethod);
            }

            if (InjectOnPropertyNameChanged && !hasCustomMethods && methods.TryGetValue("On" + propertyData.PropertyDefinition.Name + "Changed", out var defaultMethod))
            {
                propertyData.OnChangedMethods.Add(defaultMethod);
            }
        }
    }

    OnChangedMethod FindOnChangedMethod(TypeNode notifyNode, string methodName)
    {
        OnChangedMethod foundMethod = null;

        foreach (var methodDefinition in notifyNode.TypeDefinition.Methods)
        {
            if (methodDefinition.Name != methodName)
            {
                continue;
            }

            var method = CreateOnChangedMethod(notifyNode, methodDefinition, false);
            if (method == null)
            {
                continue;
            }

            if (foundMethod != null)
            {
                throw new WeavingException($"The type {notifyNode.TypeDefinition.FullName} has multiple valid overloads of a On_PropertyName_Changed method named {methodName}).");}

            foundMethod = method;
        }

        if (foundMethod == null)
        {
            throw new WeavingException($"The type {notifyNode.TypeDefinition.FullName} does not have a valid On_PropertyName_Changed method named {methodName}).");
        }

        return foundMethod;
    }

    OnChangedMethod CreateOnChangedMethod(TypeNode notifyNode, MethodDefinition methodDefinition, bool isDefaultMethod)
    {
        if (methodDefinition.IsStatic)
        {
            var message = $"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) which is static.";
            throw new WeavingException(message);
        }

        if (methodDefinition.ReturnType.FullName != "System.Void")
        {
            var message = $"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) that has a non void return value. Ensure the return type void.";
            throw new WeavingException(message);
        }

        var typeDefinitions = new Stack<TypeDefinition>();
        typeDefinitions.Push(notifyNode.TypeDefinition);

        var onChangedType = OnChangedTypes.None;

        if (IsNoArgOnChangedMethod(methodDefinition))
        {
            onChangedType = OnChangedTypes.NoArg;
        }
        else if (IsBeforeAfterOnChangedMethod(methodDefinition))
        {
            onChangedType = OnChangedTypes.BeforeAfter;
        }

        if (onChangedType != OnChangedTypes.None)
        {
            ValidateOnChangedMethod(notifyNode, methodDefinition, isDefaultMethod);

            return new OnChangedMethod
            {
                OnChangedType = onChangedType,
                MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                IsDefaultMethod = isDefaultMethod
            };
        }

        if (!EventInvokerNames.Contains(methodDefinition.Name))
        {
            EmitConditionalWarning(methodDefinition, $"Unsupported signature for a On_PropertyName_Changed method: {methodDefinition.Name} in {methodDefinition.DeclaringType.FullName}");
        }

        return null;
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

    void ValidateOnChangedMethod(TypeNode notifyNode, MethodDefinition method, bool isDefaultMethod)
    {
        if (method.IsVirtual && !method.IsNewSlot)
        {
            return;
        }

        if (!isDefaultMethod)
        {
            return;
        }
        var propertyName = method.Name.Substring("On".Length, method.Name.Length - "On".Length - "Changed".Length);

        if (notifyNode.PropertyDatas.Any(i => i.PropertyDefinition.Name == propertyName))
        {
            return;
        }
        EmitConditionalWarning(method, $"Type {method.DeclaringType.FullName} does not contain a {propertyName} property with an injected change notification, and therefore the {method.Name} method will not be called.");
    }
}
