using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;


public partial class ModuleWeaver
{
    List<string> typeLevelAttributeNames = new List<string>
    {
        "PropertyChanged.DoNotCheckEqualityAttribute",
        "PropertyChanged.DoNotNotifyAttribute",
        "PropertyChanged.DoNotSetChangedAttribute",
        "PropertyChanged.AlsoNotifyForAttribute",
        "PropertyChanged.DependsOnAttribute",
        "PropertyChanged.ImplementPropertyChangedAttribute"
    };

    List<string> assemblyLevelAttributeNames = new List<string>
    {
        "PropertyChanged.FilterTypeAttribute"
    };

    void ProcessAssembly()
    {
        var assembly = ModuleDefinition.Assembly;
        RemoveAttributes(assembly.CustomAttributes, assemblyLevelAttributeNames);
    }

    void ProcessType(TypeDefinition type)
    {
        RemoveAttributes(type.CustomAttributes, typeLevelAttributeNames);
        foreach (var property in type.Properties)
        {
            RemoveAttributes(property.CustomAttributes, typeLevelAttributeNames);
        }
        foreach (var field in type.Fields)
        {
            RemoveAttributes(field.CustomAttributes, typeLevelAttributeNames);
        }
    }

    void RemoveAttributes(Collection<CustomAttribute> customAttributes, IEnumerable<string> attributeNames)
    {
        var attributes = customAttributes
            .Where(attribute => attributeNames.Contains(attribute.Constructor.DeclaringType.FullName));

        foreach (var customAttribute in attributes.ToList())
        {
            customAttributes.Remove(customAttribute);
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