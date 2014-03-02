using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    Dictionary<string, bool> typeReferencesImplementingINotify = new Dictionary<string, bool>();

    public bool HierarchyImplementsINotify(TypeReference typeReference)
    {
        bool implementsINotify;
        var fullName = typeReference.FullName;
        if (typeReferencesImplementingINotify.TryGetValue(fullName, out implementsINotify))
        {
            return implementsINotify;
        }

        TypeDefinition typeDefinition;
        if (typeReference.IsDefinition)
        {
            typeDefinition = (TypeDefinition) typeReference;
        }
        else
        {
            typeDefinition = Resolve(typeReference);
        }

        if (HasPropertyChangedEvent(typeDefinition))
        {
            typeReferencesImplementingINotify[fullName] = true;
            return true;
        }
        if (HasPropertyChangedField(typeDefinition))
        {
            typeReferencesImplementingINotify[fullName] = true;
            return true;
        }
        var baseType = typeDefinition.BaseType;
        if (baseType == null)
        {
            typeReferencesImplementingINotify[fullName] = false;
            return false;
        }
        var baseTypeImplementsINotify = HierarchyImplementsINotify(baseType);
        typeReferencesImplementingINotify[fullName] = baseTypeImplementsINotify;
        return baseTypeImplementsINotify;
    }


    public static bool HasPropertyChangedEvent(TypeDefinition typeDefinition)
    {
        return typeDefinition.Events.Any(IsPropertyChangedEvent);
    }

    public static bool IsPropertyChangedEvent(EventDefinition eventDefinition)
    {
        return IsNamedPropertyChanged(eventDefinition) && IsPropertyChangedEventHandler(eventDefinition.EventType);
    }

    static bool IsNamedPropertyChanged(EventDefinition eventDefinition)
    {
        return eventDefinition.Name == "PropertyChanged" ||
               eventDefinition.Name == "System.ComponentModel.INotifyPropertyChanged.PropertyChanged" ||
               eventDefinition.Name == "Windows.UI.Xaml.Data.PropertyChanged";
    }

    public static bool IsPropertyChangedEventHandler(TypeReference typeReference)
    {
        return typeReference.FullName == "System.ComponentModel.PropertyChangedEventHandler" ||
               typeReference.FullName == "Windows.UI.Xaml.Data.PropertyChangedEventHandler" ||
               typeReference.FullName == "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable`1<Windows.UI.Xaml.Data.PropertyChangedEventHandler>";
    }

    static bool HasPropertyChangedField(TypeDefinition typeDefinition)
    {
        foreach (var fieldType in typeDefinition.Fields.Select(x => x.FieldType))
        {
            if (fieldType.FullName == "Microsoft.FSharp.Control.FSharpEvent`2<System.ComponentModel.PropertyChangedEventHandler,System.ComponentModel.PropertyChangedEventArgs>")
            {
                return true;
            }
            if (fieldType.FullName == "Microsoft.FSharp.Control.FSharpEvent`2<Windows.UI.Xaml.Data.PropertyChangedEventHandler,Windows.UI.Xaml.Data.PropertyChangedEventArgs>")
            {
                return true;
            }
        }
        return false;
    }
}