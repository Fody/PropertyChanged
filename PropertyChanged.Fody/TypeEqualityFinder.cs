using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class TypeEqualityFinder
{
    ModuleWeaver moduleWeaver;
    MsCoreReferenceFinder msCoreReferenceFinder;
    TypeResolver typeResolver;
    Dictionary<string, MethodReference> methodCache;
    public MethodReference StringEquals;
    public int OrdinalStringComparison;

    public TypeEqualityFinder(ModuleWeaver moduleWeaver, MsCoreReferenceFinder msCoreReferenceFinder, TypeResolver typeResolver)
    {
        methodCache = new Dictionary<string, MethodReference>();

        this.moduleWeaver = moduleWeaver;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.typeResolver = typeResolver;


        var stringEquals = moduleWeaver
            .ModuleDefinition
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
        StringEquals = moduleWeaver.ModuleDefinition.Import(stringEquals);
        OrdinalStringComparison = (int) StringEquals
                                            .Parameters[2]
                                            .ParameterType
                                            .Resolve()
                                            .Fields
                                            .First(x => x.Name == "Ordinal")
                                            .Constant;
    }



    public MethodReference Find(TypeReference typeDefinition)
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
                var typeWrappedByNullable = ((GenericInstanceType) typeDefinition).GenericArguments.First();
                var genericInstanceMethod = new GenericInstanceMethod(msCoreReferenceFinder.NullableEqualsMethod);
                genericInstanceMethod.GenericArguments.Add(typeWrappedByNullable);
                return moduleWeaver.ModuleDefinition.Import(genericInstanceMethod);
            }
        }
        var equality = GetStaticEquality(typeDefinition);
        if (equality != null)
        {
            return moduleWeaver.ModuleDefinition.Import(equality);
        }
        return null;
    }

    MethodReference GetStaticEquality(TypeReference typeReference)
    {
        var typeDefinition = typeResolver.Resolve(typeReference);
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