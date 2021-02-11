using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            notifyNode.OnChangedMethods = GetOnChangedMethods(notifyNode);
            ProcessOnChangedMethods(notifyNode.Nodes);
        }
    }

    List<OnChangedMethod> GetOnChangedMethods(TypeNode notifyNode)
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

        foreach (var propertyDefinition in notifyNode.AllProperties)
        {
            var hasCustomMethods = false;

            foreach (var attribute in propertyDefinition.CustomAttributes.GetAttributes("PropertyChanged.OnChangedMethodAttribute"))
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

                onChangedMethod.Properties.Add(propertyDefinition);
            }

            if (InjectOnPropertyNameChanged && !hasCustomMethods && methods.TryGetValue("On" + propertyDefinition.Name + "Changed", out var defaultMethod))
            {
                defaultMethod.Properties.Add(propertyDefinition);
            }
        }

        foreach (var method in methods.Values)
        {
            ValidateOnChangedMethod(notifyNode, method);

        }

        return methods.Values.ToList();
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
                throw new WeavingException($"The type {notifyNode.TypeDefinition.FullName} has multiple valid overloads of a On_PropertyName_Changed method named {methodName}).");
            }

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
            if (!SuppressOnPropertyNameChangedWarning)
            {
                EmitConditionalWarning(methodDefinition, $"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) which is static.");
            }
            return null;
        }

        if (methodDefinition.ReturnType.FullName != "System.Void")
        {
            if (!SuppressOnPropertyNameChangedWarning)
            {
                EmitConditionalWarning(methodDefinition, $"The type {notifyNode.TypeDefinition.FullName} has a On_PropertyName_Changed method ({methodDefinition.Name}) that has a non void return value. Ensure the return type void.");
            }
            return null;
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
            return new OnChangedMethod
            {
                OnChangedType = onChangedType,
                MethodDefinition = methodDefinition,
                MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                IsDefaultMethod = isDefaultMethod
            };
        }

        if (!EventInvokerNames.Contains(methodDefinition.Name) && !SuppressOnPropertyNameChangedWarning)
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

    void ValidateOnChangedMethod(TypeNode notifyNode, OnChangedMethod onChangedMethod)
    {
        var method = onChangedMethod.MethodDefinition;

        if (method.IsVirtual && !method.IsNewSlot)
        {
            return;
        }

        if (!onChangedMethod.IsDefaultMethod)
        {
            return;
        }

        if (onChangedMethod.Properties.Any())
        {
            return;
        }
        
        if (!SuppressOnPropertyNameChangedWarning)
        {
            // var propertyName = method.Name.Substring("On".Length, method.Name.Length - "On".Length - "Changed".Length);
            EmitConditionalWarning(method, $"Type {method.DeclaringType.FullName} contains a method {method.Name} which will not be called.");
        }
    }
}