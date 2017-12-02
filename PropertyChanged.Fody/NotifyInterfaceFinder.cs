using System.Collections.Generic;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, bool> typesImplementingINotify = new Dictionary<string, bool>();

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
            typeDefinition = Resolve(typeReference);
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

    public static bool IsPropertyChangedEventHandler(TypeReference typeReference)
    {
        return typeReference.FullName == "System.ComponentModel.PropertyChangedEventHandler" ||
               typeReference.FullName == "Windows.UI.Xaml.Data.PropertyChangedEventHandler" ||
               typeReference.FullName == "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable`1<Windows.UI.Xaml.Data.PropertyChangedEventHandler>";
    }
}