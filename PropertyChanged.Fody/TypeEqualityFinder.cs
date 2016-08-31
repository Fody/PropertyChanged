using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<TypeReference, EqualityComparerRef> equalityComparerCache = new Dictionary<TypeReference, EqualityComparerRef>();

    public EqualityComparerRef GetEqualityComparer(TypeReference targetType)
    {
        EqualityComparerRef result;
        if (!equalityComparerCache.TryGetValue(targetType, out result))
        {
            var equalityComparerType = new GenericInstanceType(ModuleDefinition.ImportReference(typeof(EqualityComparer<>)));
            equalityComparerType.GenericArguments.Add(ModuleDefinition.ImportReference(targetType));

            var equalityComparerRef = ModuleDefinition.ImportReference(equalityComparerType);
            var equalityComparerDef = equalityComparerRef.Resolve();

            var defaultMethodDef = equalityComparerDef.Methods.First(m => m.Name == "get_Default");
            var equalsMethodDef = FindNamedMethod(equalityComparerDef, "Equals", equalityComparerDef.GenericParameters[0]);

            var defaultMethodRef = ModuleDefinition.ImportReference(defaultMethodDef);
            var equalsMethodRef = ModuleDefinition.ImportReference(equalsMethodDef);

            defaultMethodRef.DeclaringType = equalityComparerRef;
            equalsMethodRef.DeclaringType = equalityComparerRef;

            equalityComparerCache.Add(targetType, result = new EqualityComparerRef(defaultMethodRef, equalsMethodRef));
        }

        return result;
    }

    static MethodReference FindNamedMethod(TypeDefinition typeDefinition, string methodName, TypeReference parameterType)
    {
        return typeDefinition.Methods
            .First(x =>
                x.Name == methodName &&
                !x.IsStatic &&
                x.ReturnType.Name == "Boolean" &&
                x.HasParameters &&
                x.Parameters.Count == 2 &&
                MatchParameter(x.Parameters[0], parameterType) &&
                MatchParameter(x.Parameters[1], parameterType)
            );
    }

    static bool MatchParameter(ParameterDefinition parameter, TypeReference typeMatch)
    {
        if (parameter.ParameterType == typeMatch)
            return true;

        if (parameter.ParameterType.IsGenericInstance && typeMatch.IsGenericInstance)
            return parameter.ParameterType.Resolve() == typeMatch.Resolve();

        return false;
    }
}