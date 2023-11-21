using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public static class CecilExtensions
{
    public static string GetName(this PropertyDefinition propertyDefinition)
    {
        return $"{propertyDefinition.DeclaringType.FullName}.{propertyDefinition.Name}";
    }

    public static bool IsCallToBaseMethod(this Instruction instruction, MethodDefinition method)
    {
        return instruction.OpCode == OpCodes.Call && instruction.IsCallToMethod(method);
    }

    public static bool IsCallToMethod(this Instruction instruction, MethodDefinition method)
    {
        if (!instruction.OpCode.IsCall())
        {
            return false;
        }

        if (instruction.Operand is not MethodReference methodReference)
        {
            return false;
        }

        if (methodReference.Name != method.Name)
        {
            return false;
        }

        if (methodReference.Resolve() != method)
        {
            return false;
        }

        return true;
    }

    public static bool IsCallToMethod(this Instruction instruction, string methodName, out int propertyNameIndex)
    {
        propertyNameIndex = 1;
        if (!instruction.OpCode.IsCall())
        {
            return false;
        }

        if (instruction.Operand is not MethodReference methodReference)
        {
            return false;
        }

        if (methodReference.Name != methodName)
        {
            return false;
        }

        var parameterDefinition = methodReference.Parameters.FirstOrDefault(_ => _.Name == "propertyName");
        if (parameterDefinition != null)
        {
            propertyNameIndex = methodReference.Parameters.Count - parameterDefinition.Index;
        }

        return true;
    }

    public static bool IsCall(this OpCode opCode)
    {
        return opCode.Code is Code.Call or Code.Callvirt;
    }

    public static TypeReference GetGeneric(this TypeReference reference)
    {
        if (!reference.HasGenericParameters)
        {
            return reference;
        }

        var genericType = new GenericInstanceType(reference);
        foreach (var parameter in reference.GenericParameters)
        {
            genericType.GenericArguments.Add(parameter);
        }

        return genericType;
    }

    public static FieldReference GetGeneric(this FieldDefinition definition)
    {
        if (!definition.DeclaringType.HasGenericParameters)
        {
            return definition;
        }

        var declaringType = new GenericInstanceType(definition.DeclaringType);
        foreach (var parameter in definition.DeclaringType.GenericParameters)
        {
            declaringType.GenericArguments.Add(parameter);
        }

        return new(definition.Name, definition.FieldType, declaringType);
    }

    public static MethodReference GetGeneric(this MethodReference reference)
    {
        if (!reference.DeclaringType.HasGenericParameters)
        {
            return reference;
        }

        var declaringType = new GenericInstanceType(reference.DeclaringType);
        foreach (var parameter in reference.DeclaringType.GenericParameters)
        {
            declaringType.GenericArguments.Add(parameter);
        }

        var methodReference = new MethodReference(reference.Name, reference.MethodReturnType.ReturnType, declaringType);
        foreach (var parameterDefinition in reference.Parameters)
        {
            methodReference.Parameters.Add(parameterDefinition);
        }

        methodReference.HasThis = reference.HasThis;
        return methodReference;
    }

    public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
    {
        var reference = new MethodReference(self.Name, self.ReturnType)
        {
            HasThis = self.HasThis,
            ExplicitThis = self.ExplicitThis,
            DeclaringType = self.DeclaringType.MakeGenericInstanceType(arguments),
            CallingConvention = self.CallingConvention,
        };

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new(parameter.ParameterType));
        }

        foreach (var genericParameter in self.GenericParameters)
        {
            reference.GenericParameters.Add(new(genericParameter.Name, reference));
        }

        return reference;
    }

    public static IEnumerable<CustomAttribute> GetAllCustomAttributes(this TypeDefinition typeDefinition)
    {
        foreach (var attribute in typeDefinition.CustomAttributes)
        {
            yield return attribute;
        }

        if (typeDefinition.BaseType is not TypeDefinition baseDefinition)
        {
            yield break;
        }

        foreach (var attribute in baseDefinition.GetAllCustomAttributes())
        {
            yield return attribute;
        }
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this IEnumerable<CustomAttribute> attributes, string attributeName)
    {
        return attributes.Where(attribute => attribute.Constructor.DeclaringType.FullName == attributeName);
    }

    public static CustomAttribute GetAttribute(this IEnumerable<CustomAttribute> attributes, string attributeName)
    {
        return attributes.FirstOrDefault(attribute => attribute.Constructor.DeclaringType.FullName == attributeName);
    }

    public static bool ContainsAttribute(this IEnumerable<CustomAttribute> attributes, string attributeName)
    {
        return attributes.Any(attribute => attribute.Constructor.DeclaringType.FullName == attributeName);
    }

    public static IEnumerable<TypeReference> GetAllInterfaces(this TypeDefinition type)
    {
        while (type != null)
        {
            if (type.HasInterfaces)
            {
                foreach (var iface in type.Interfaces)
                {
                    yield return iface.InterfaceType;
                }
            }

            type = type.BaseType?.Resolve();
        }
    }

    public static bool GetBaseMethod(this MethodDefinition method, out MethodDefinition baseMethod)
    {
        baseMethod = method.GetBaseMethod();
        return baseMethod != null
            && baseMethod != method; // cecil's GetBaseMethod() returns self if the method has no base method...
    }

    public static IEnumerable<MethodDefinition> GetSelfAndBaseMethods(this MethodDefinition method)
    {
        yield return method;

        while (method.GetBaseMethod(out method))
        {
            yield return method;
        }
    }

    public static OpCode GetCallOpCode(this TypeReference type)
        => type.IsValueType ? OpCodes.Call : OpCodes.Callvirt;

    public static void AddConditionalBoxInstructions(this ICollection<Instruction> instructions, TypeReference type)
    {
        if (type.IsValueType)
        {
            var genericType = type.GetGeneric();
            instructions.Add(Instruction.Create(OpCodes.Ldobj, genericType));
            instructions.Add(Instruction.Create(OpCodes.Box, genericType));
        }
    }
}
