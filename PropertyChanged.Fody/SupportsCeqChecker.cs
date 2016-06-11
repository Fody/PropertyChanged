using System;
using System.Collections.Generic;
using Mono.Cecil;

public static class SupportsCeqChecker
{

    static List<string> ceqStructNames;

    static SupportsCeqChecker()
    {
        ceqStructNames = new List<string>
                             {
                                 typeof (int).Name,
                                 typeof (uint).Name,
                                 typeof (long).Name,
                                 typeof (ulong).Name,
                                 typeof (float).Name,
                                 typeof (double).Name,
                                 typeof (bool).Name,
                                 typeof (short).Name,
                                 typeof (ushort).Name,
                                 typeof (byte).Name,
                                 typeof (sbyte).Name,
                                 typeof (char).Name,
                             };
    }

    public static bool SupportsCeq(this TypeReference typeReference)
    {
        if (ceqStructNames.Contains(typeReference.Name))
        {
            return true;
        }
        if (typeReference.IsArray)
        {
            return false;
        }
        if (typeReference.ContainsGenericParameter)
        {
            return false;
        }
        var typeDefinition = typeReference.Resolve();
        if (typeDefinition == null)
        {
            throw new Exception($"Could not resolve '{typeReference.FullName}'.");
        }
        if (typeDefinition.IsEnum)
        {
            return true;
        }
        return !typeDefinition.IsValueType;
    }
}