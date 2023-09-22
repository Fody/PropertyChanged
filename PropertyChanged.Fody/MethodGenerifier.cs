using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetMethodReference(Stack<TypeDefinition> typeDefinitions, MethodDefinition methodDefinition)
    {
        var methodReference = ModuleDefinition.ImportReference(methodDefinition).GetGeneric();

        if (methodReference.DeclaringType.IsGenericInstance && typeDefinitions.Count > 1)
        {
            methodReference = MakeGeneric(typeDefinitions.Last().BaseType, methodReference);
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
            reference.Parameters.Add(new(parameter.ParameterType));
        }

        return reference;
    }
}
