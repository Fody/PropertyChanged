using System;
using System.Collections.Generic;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, TypeDefinition> definitions = new();

    public TypeDefinition Resolve(TypeReference reference)
    {
        if (definitions.TryGetValue(reference.FullName, out var definition))
        {
            return definition;
        }
        return definitions[reference.FullName] = InnerResolve(reference);
    }

    static TypeDefinition InnerResolve(TypeReference reference)
    {
        TypeDefinition result;

        try
        {
            result = reference.Resolve();
        }
        catch (Exception exception)
        {
            throw new($"Could not resolve '{reference.FullName}'.", exception);
        }

        if (result == null)
        {
            throw new($"Could not resolve '{reference.FullName}'. TypeReference.Resolve returned null.");
        }

        return result;
    }
}