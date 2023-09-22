using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, bool> typesImplementingINotify = new();

    public bool HierarchyImplementsINotify(TypeReference typeReference)
    {
        var fullName = typeReference.FullName;
        if (typesImplementingINotify.TryGetValue(fullName, out var implementsINotify))
        {
            return implementsINotify;
        }

        TypeDefinition typeDefinition;
        if (typeReference.IsDefinition)
        {
            typeDefinition = (TypeDefinition)typeReference;
        }
        else
        {
            try
            {
                typeDefinition = Resolve(typeReference);
            }
            catch (Exception ex)
            {
                EmitWarning($"Ignoring type {fullName} in type hierarchy => {ex.Message}");
                return false;
            }
        }

        foreach (var interfaceImplementation in typeDefinition.Interfaces)
        {
            if (interfaceImplementation.InterfaceType.Name == "INotifyPropertyChanged")
            {
                typesImplementingINotify[fullName] = true;
                return true;
            }
        }

        var baseType = typeDefinition.BaseType;
        if (baseType == null)
        {
            typesImplementingINotify[fullName] = false;
            return false;
        }

        var baseTypeImplementsINotify = HierarchyImplementsINotify(baseType);
        typesImplementingINotify[fullName] = baseTypeImplementsINotify;
        return baseTypeImplementsINotify;
    }

    static bool HasGeneratedPropertyChangedEvent(TypeDefinition typeDefinition)
    {
        if (!typeDefinition.HasEvents)
        {
            return false;
        }

        var evt = typeDefinition.Events.FirstOrDefault(i => i.Name == "PropertyChanged");
        return evt?.CustomAttributes.ContainsAttribute("System.CodeDom.Compiler.GeneratedCodeAttribute") is true;
    }
}
