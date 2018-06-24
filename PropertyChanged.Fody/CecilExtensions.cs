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

    public static bool IsCallToMethod(this Instruction instruction, string methodName, out int propertyNameIndex)
    {
        propertyNameIndex = 1;
        if (!instruction.OpCode.IsCall())
        {
            return false;
        }

        if (!(instruction.Operand is MethodReference methodReference))
        {
            return false;
        }

        if (methodReference.Name != methodName)
        {
            return false;
        }

        var parameterDefinition = methodReference.Parameters.FirstOrDefault(x => x.Name == "propertyName");
        if (parameterDefinition != null)
        {
            propertyNameIndex = methodReference.Parameters.Count - parameterDefinition.Index;
        }

        return true;
    }

    public static bool IsCall(this OpCode opCode)
    {
        return opCode.Code == Code.Call || opCode.Code == Code.Callvirt;
    }

    public static FieldReference GetGeneric(this FieldDefinition definition)
    {
        if (definition.DeclaringType.HasGenericParameters)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var parameter in definition.DeclaringType.GenericParameters)
            {
                declaringType.GenericArguments.Add(parameter);
            }

            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        return definition;
    }

    public static MethodReference GetGeneric(this MethodReference reference)
    {
        if (reference.DeclaringType.HasGenericParameters)
        {
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

        return reference;
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
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

        foreach (var genericParameter in self.GenericParameters)
            reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));

        return reference;
    }

    public static IEnumerable<CustomAttribute> GetAllCustomAttributes(this TypeDefinition typeDefinition)
    {
        foreach (var attribute in typeDefinition.CustomAttributes)
        {
            yield return attribute;
        }

        if (!(typeDefinition.BaseType is TypeDefinition baseDefinition))
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
}