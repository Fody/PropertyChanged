using System.Collections.Generic;
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
            foreach (var p in self.GenericParameters)
            {
                var gp = new GenericParameter(p.Name, self);
                reference.GenericParameters.Add(gp);
            }

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

}