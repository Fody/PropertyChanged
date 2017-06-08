﻿using System.Collections.Generic;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetMethodReference(Stack<TypeDefinition> typeDefinitions, MethodDefinition methodDefinition)
    {
        var methodReference = ModuleDefinition.ImportReference(methodDefinition).GetGeneric();
        typeDefinitions.Pop();
        while (typeDefinitions.Count > 0)
        {
            var definition = typeDefinitions.Pop();
            methodReference = MakeGeneric(definition.BaseType, methodReference);
        }
        return methodReference;
    }

    public static MethodReference MakeGeneric(TypeReference declaringType, MethodReference self)
    {
        var reference = new MethodReference(self.Name, self.ReturnType)
                            {
                                DeclaringType = declaringType,
                                HasThis = self.HasThis,
                                ExplicitThis = self.ExplicitThis,
                                CallingConvention = self.CallingConvention,
                            };

        if (self.HasGenericParameters)
        {
            ImportGenericParameters(reference, self);
        }

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

    internal static void ImportGenericParameters(IGenericParameterProvider imported, IGenericParameterProvider original)
    {
        var parameters = original.GenericParameters;
        var importedParameters = imported.GenericParameters;

        foreach (var t in parameters)
            importedParameters.Add(new GenericParameter(t.Name, imported));
    }
}