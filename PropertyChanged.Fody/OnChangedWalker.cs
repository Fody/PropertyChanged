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
            notifyNode.OnChangedMethods = GetOnChangedMethods(notifyNode);
            ProcessOnChangedMethods(notifyNode.Nodes);
        }
    }

    ICollection<OnChangedMethod> GetOnChangedMethods(TypeNode notifyNode)
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

        foreach (var propertyDefinition in notifyNode.DeclaredProperties)
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

                if (onChangedMethod.OnChangedType == OnChangedTypes.BeforeAfterTyped && onChangedMethod.ArgumentTypeFullName != propertyDefinition.PropertyType.FullName)
                {
                    if (!SuppressOnPropertyNameChangedWarning)
                    {
                        var methodDefinition = onChangedMethod.MethodDefinition;
                        EmitConditionalWarning(methodDefinition, $"Unsupported signature for a On_PropertyName_Changed method: {methodDefinition.Name} in {methodDefinition.DeclaringType.FullName}");
                    }

                    continue;
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

        return methods.Values;
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

        var onChangedType = GetBeforeAfterOnChangedMethodType(methodDefinition, out var argumentTypeFullName);

        if (onChangedType != OnChangedTypes.None)
        {
            return new()
            {
                OnChangedType = onChangedType,
                MethodDefinition = methodDefinition,
                MethodReference = GetMethodReference(typeDefinitions, methodDefinition),
                IsDefaultMethod = isDefaultMethod,
                ArgumentTypeFullName = argumentTypeFullName
            };
        }

        if (!EventInvokerNames.Contains(methodDefinition.Name) && !SuppressOnPropertyNameChangedWarning)
        {
            EmitConditionalWarning(methodDefinition, $"Unsupported signature for a On_PropertyName_Changed method: {methodDefinition.Name} in {methodDefinition.DeclaringType.FullName}");
        }

        return null;
    }

    static OnChangedTypes GetBeforeAfterOnChangedMethodType(MethodDefinition method, out string argumentTypeFullName)
    {
        argumentTypeFullName = null;

        var parameters = method.Parameters;

        if (parameters.Count == 0)
            return OnChangedTypes.NoArg;

        if (parameters.Count != 2)
            return OnChangedTypes.None;

        argumentTypeFullName = parameters[0].ParameterType.FullName;

        if (argumentTypeFullName != parameters[1].ParameterType.FullName)
            return OnChangedTypes.None;

        if (argumentTypeFullName == "System.Object")
        {
            argumentTypeFullName = null;
            return OnChangedTypes.BeforeAfter;
        }

        return OnChangedTypes.BeforeAfterTyped;
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

        bool PropertyWillBeNotified(PropertyDefinition p)
        {
            return notifyNode.PropertyDatas
                .Any(pd => pd.PropertyDefinition == p || pd.AlsoNotifyFor.Contains(p));
        }

        if (onChangedMethod.Properties.Any(PropertyWillBeNotified))
        {
            return;
        }

        if (!SuppressOnPropertyNameChangedWarning)
        {
            EmitConditionalWarning(method, GetMethodWarning(notifyNode, onChangedMethod));
        }
    }

    static string GetMethodWarning(TypeNode notifyNode, OnChangedMethod onChangedMethod)
    {
        var method = onChangedMethod.MethodDefinition;
        var methodRef = onChangedMethod.MethodReference;

        var propertyName = method.Name.Substring("On".Length, method.Name.Length - "On".Length - "Changed".Length);

        var baseMessage = $"Type {method.DeclaringType.FullName} contains a method {method.Name} which will not be called";

        var foundProperty = GetProperty(methodRef, propertyName);

        if (foundProperty == null)
            return $"{baseMessage} as {propertyName} is not found.";

        if (foundProperty.DeclaringType != method.DeclaringType)
            return $"{baseMessage} as {propertyName} is declared on base class {foundProperty.DeclaringType.Name}.";

        if (foundProperty.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotNotifyAttribute"))
            return $"{baseMessage} as {propertyName} is attributed with [DoNotNotify].";

        if (foundProperty.CustomAttributes.ContainsAttribute("PropertyChanged.OnChangedMethodAttribute"))
            return $"{baseMessage} as {propertyName} is attributed with an alternative [OnChangedMethod].";

        return $"{baseMessage}.";
    }

    static PropertyDefinition GetProperty(MethodReference methodRef, string propertyName)
    {
        var type = methodRef.DeclaringType;
        while (type != null)
        {
            var found = type.Resolve().Properties.FirstOrDefault(p => p.Name == propertyName);
            if (found != null)
            {
                return found;
            }
            type = type.Resolve().BaseType;
        }

        return null;
    }

}