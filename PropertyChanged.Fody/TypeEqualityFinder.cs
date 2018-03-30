﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    Dictionary<string, MethodReference> methodCache;
    public int OrdinalStringComparison;

    public void FindComparisonMethods()
    {
        methodCache = new Dictionary<string, MethodReference>();

        OrdinalStringComparison = (int) StringEquals
            .Parameters[2]
            .ParameterType
            .Resolve()
            .Fields
            .First(x => x.Name == "Ordinal")
            .Constant;

        foreach (var node in NotifyNodes)
        {
            foreach (var data in node.PropertyDatas)
            {
                data.EqualsMethod = FindTypeEquality(data);
            }
        }

        methodCache = null;
    }

    MethodReference FindTypeEquality(PropertyData propertyData)
    {
        var typeDefinition = propertyData.PropertyDefinition.PropertyType;
        var fullName = typeDefinition.FullName;
        if (methodCache.TryGetValue(fullName, out var methodReference))
        {
            return methodReference;
        }

        var equality = GetEquality(typeDefinition);
        methodCache[fullName] = equality;
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

                if (typeWrappedByNullable.IsGenericParameter)
                {
                    return ModuleDefinition.ImportReference(genericInstanceMethod, typeWrappedByNullable.DeclaringType);
                }
                return ModuleDefinition.ImportReference(genericInstanceMethod);
            }
        }
        
        return GetStaticEquality(typeDefinition);
    }

    MethodReference GetStaticEquality(TypeReference typeReference)
    {
        var typeDefinition = Resolve(typeReference);
        if (typeDefinition.IsInterface)
        {
            return null;
        }

        MethodReference equality = null;
        var typesChecked = new List<string>();

        if (UseStaticEqualsFromBase)
        {
            while (equality == null && 
                   typeReference != null && 
                   typeReference.FullName != typeof(object).FullName &&
                   !methodCache.TryGetValue(typeReference.FullName, out equality))
            {
                typesChecked.Add(typeReference.FullName);
                equality = FindNamedMethod(typeReference);
                if (equality == null)
                    typeReference = GetBaseType(typeReference);
            }
        }
        else
            equality = FindNamedMethod(typeReference);

        if (equality != null)
            equality = ModuleDefinition.ImportReference(equality);

        typesChecked.ForEach(typeName => methodCache[typeName] = equality);

        return equality;
    }

    TypeReference GetBaseType(TypeReference typeReference)
    {
        var typeDef = typeReference as TypeDefinition ?? typeReference.Resolve();
        var baseType = typeDef?.BaseType;

        if (baseType == null)
            return null;

        if (baseType.IsGenericInstance && typeReference.IsGenericInstance)
        {
            //currently we have something like: baseType = BaseClass<T>, typeReference = Class<int> (where the class inherits from BaseClass<T> and int is the parameter for T).
            //We want BaseClass<int> -> map generic arguments to the actual parameter types
            var genericBaseType = (GenericInstanceType)baseType;
            var genericTypeRef = (GenericInstanceType)typeReference;

            //create a map from the type reference (child class): generic argument name -> type
            var typeRefDict = new Dictionary<string, TypeReference>();
            var typeRefParams = genericTypeRef.ElementType.Resolve().GenericParameters;
            for (int i = 0; i < typeRefParams.Count; i++)
            {
                string paramName = typeRefParams[i].FullName;
                TypeReference paramType = genericTypeRef.GenericArguments[i];
                typeRefDict[paramName] = paramType;
            }
            
            //apply to base type
            //note: even though the base class may have different argument names in the source code, the argument names of the inheriting class are used in the GenericArguments
            //thus we can directly map them.
            var baseTypeArgs = genericBaseType.GenericArguments.Select(arg =>
            {
                if (typeRefDict.TryGetValue(arg.FullName, out TypeReference t))
                    return t;

                return arg;
            }).ToArray();

            baseType = genericBaseType.ElementType.MakeGenericInstanceType(baseTypeArgs);
        }

        return baseType;
    }

    public static MethodReference FindNamedMethod(TypeReference typeReference)
    {
        var typeDefinition = typeReference.Resolve();
        var equalsMethod = FindNamedMethod(typeDefinition, "Equals", typeReference);
        if (equalsMethod == null)
        {
            equalsMethod = FindNamedMethod(typeDefinition, "op_Equality", typeReference);
        }
        if (equalsMethod != null && typeReference.IsGenericInstance)
        {
            var genericType = new GenericInstanceType(equalsMethod.DeclaringType);
            foreach (var argument in ((GenericInstanceType) typeReference).GenericArguments)
            {
                genericType.GenericArguments.Add(argument);
            }
            equalsMethod = MakeGeneric(genericType, equalsMethod);
        }
        return equalsMethod;
    }

    static MethodReference FindNamedMethod(TypeDefinition typeDefinition, string methodName, TypeReference parameterType)
    {
        MethodReference reference =  typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName &&
                                                          x.IsStatic &&
                                                          x.ReturnType.Name == "Boolean" &&
                                                          x.HasParameters &&
                                                          x.Parameters.Count == 2 &&
                                                          MatchParameter(x.Parameters[0], parameterType) &&
                                                          MatchParameter(x.Parameters[1], parameterType));

        if (reference == null && typeDefinition != parameterType)
            reference = FindNamedMethod(typeDefinition, methodName, typeDefinition);

        return reference;
    }

    static bool MatchParameter(ParameterDefinition parameter, TypeReference typeMatch)
    {
        if (parameter.ParameterType == typeMatch)
        {
            return true;
        }

        if (parameter.ParameterType.IsGenericInstance && typeMatch.IsGenericInstance)
        {
            return parameter.ParameterType.Resolve() == typeMatch.Resolve();
        }

        return false;
    }
}