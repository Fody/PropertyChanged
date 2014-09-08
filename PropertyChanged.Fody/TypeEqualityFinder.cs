using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, MethodReference> methodCache;
    public MethodReference StringEquals;
    public int OrdinalStringComparison;

    public void FindComparisonMethods()
    {
        methodCache = new Dictionary<string, MethodReference>();


        var stringEquals = ModuleDefinition
            .TypeSystem
            .String
            .Resolve()
            .Methods
            .First(x => x.IsStatic && 
                x.Name == "Equals" && 
                x.Parameters.Count == 3 &&
                x.Parameters[0].ParameterType.Name == "String" && 
                x.Parameters[1].ParameterType.Name == "String" && 
                x.Parameters[2].ParameterType.Name == "StringComparison");
        StringEquals = ModuleDefinition.Import(stringEquals);
        OrdinalStringComparison = (int) StringEquals
                                            .Parameters[2]
                                            .ParameterType
                                            .Resolve()
                                            .Fields
                                            .First(x => x.Name == "Ordinal")
                                            .Constant;
    }



    public MethodReference FindTypeEquality(TypeReference typeDefinition)
    {
        MethodReference methodReference;
        var fullName = typeDefinition.FullName;
        if (methodCache.TryGetValue(fullName, out methodReference))
        {
            return methodReference;
        }

        var equality = GetEquality(typeDefinition);
        methodCache.Add(fullName, equality);
        return equality;
    }

    MethodReference GetEquality(TypeReference typeDefinition)
    {

        if (typeDefinition.IsArray)
        {
            return null;
        }
        if (typeDefinition.IsGenericParameter)
        {
            return null;
        }
        if (typeDefinition.Namespace.StartsWith("System.Collections"))
        {
            return null;
        }
        if (typeDefinition.IsGenericInstance)
        {
            if (typeDefinition.FullName.StartsWith("System.Nullable"))
            {
                var genericInstanceMethod = new GenericInstanceMethod(NullableEqualsMethod);
                var typeWrappedByNullable = ((GenericInstanceType) typeDefinition).GenericArguments.First();
                genericInstanceMethod.GenericArguments.Add(typeWrappedByNullable);

                return typeWrappedByNullable.IsGenericParameter ? ModuleDefinition.Import(genericInstanceMethod, typeWrappedByNullable.DeclaringType) : ModuleDefinition.Import(genericInstanceMethod);
            }
        }
        var equality = GetStaticEquality(typeDefinition);
        if (equality != null)
        {
            return ModuleDefinition.Import(equality);
        }
        return null;
    }

    MethodReference GetStaticEquality(TypeReference typeReference)
    {
        var typeDefinition = Resolve(typeReference);
        if (typeDefinition.IsInterface)
        {
            return null;
        }

        return FindNamedMethod(typeDefinition);
    }

    public static MethodReference FindNamedMethod(TypeDefinition typeDefinition)
    {
        var equalsMethod = FindNamedMethod(typeDefinition, "Equals");
        if (equalsMethod == null)
        {
            return FindNamedMethod(typeDefinition, "op_Equality");
        }
        return equalsMethod;
    }

    static MethodReference FindNamedMethod(TypeDefinition typeDefinition, string methodName)
    {
        return typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName &&
                                                          x.IsStatic &&
                                                          x.ReturnType.Name == "Boolean" &&
                                                          x.HasParameters &&
                                                          x.Parameters.Count == 2 &&
                                                          x.Parameters[0].ParameterType == typeDefinition &&
                                                          x.Parameters[1].ParameterType == typeDefinition);
    }
}