using System.Collections.Generic;
using Mono.Cecil;

public class MethodGenerifier
{
    ModuleWeaver moduleWeaver;

    public MethodGenerifier(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
    }

    public MethodReference GetMethodReference(Stack<TypeDefinition> typeDefinitions, MethodDefinition methodDefinition)
    {
        var methodReference = moduleWeaver.ModuleDefinition.Import(methodDefinition).GetGeneric();
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

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

}