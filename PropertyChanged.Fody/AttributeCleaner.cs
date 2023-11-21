using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    HashSet<string> typeLevelAttributeNames =
    [
        "PropertyChanged.DoNotCheckEqualityAttribute",
        "PropertyChanged.DoNotNotifyAttribute",
        "PropertyChanged.DoNotSetChangedAttribute",
        "PropertyChanged.AlsoNotifyForAttribute",
        "PropertyChanged.DependsOnAttribute",
        "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute",
        "PropertyChanged.SuppressPropertyChangedWarningsAttribute",
        "PropertyChanged.OnChangedMethodAttribute"
    ];

    HashSet<string> assemblyLevelAttributeNames = ["PropertyChanged.FilterTypeAttribute"];

    void ProcessAssembly()
    {
        var assembly = ModuleDefinition.Assembly;
        RemoveAttributes(assembly, assemblyLevelAttributeNames);
    }

    void ProcessType(TypeDefinition type)
    {
        RemoveAttributes(type, typeLevelAttributeNames);

        foreach (var property in type.Properties)
        {
            RemoveAttributes(property, typeLevelAttributeNames);
        }

        foreach (var field in type.Fields)
        {
            RemoveAttributes(field, typeLevelAttributeNames);
        }

        foreach (var method in type.Methods)
        {
            RemoveAttributes(method, typeLevelAttributeNames);
        }
    }

    static void RemoveAttributes(ICustomAttributeProvider member, IEnumerable<string> attributeNames)
    {
        if (!member.HasCustomAttributes)
        {
            return;
        }

        var attributes = member.CustomAttributes
            .Where(attribute => attributeNames.Contains(attribute.Constructor.DeclaringType.FullName));

        foreach (var customAttribute in attributes.ToList())
        {
            member.CustomAttributes.Remove(customAttribute);
        }
    }

    public void CleanAttributes()
    {
        foreach (var type in ModuleDefinition.GetTypes())
        {
            ProcessType(type);
        }

        ProcessAssembly();
    }
}
