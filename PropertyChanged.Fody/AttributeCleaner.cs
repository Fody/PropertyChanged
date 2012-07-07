using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;


public class AttributeCleaner
{
    ModuleWeaver moduleWeaver;
    AllTypesFinder allTypesFinder;
    List<string> propertyAttributeNames;

    public AttributeCleaner(ModuleWeaver moduleWeaver, AllTypesFinder allTypesFinder)
    {
        propertyAttributeNames = new List<string> { "PropertyChanged.DoNotNotifyAttribute", "PropertyChanged.DoNotSetChangedAttribute", "PropertyChanged.AlsoNotifyForAttribute", "PropertyChanged.DependsOnAttribute" };
        this.moduleWeaver = moduleWeaver;
        this.allTypesFinder = allTypesFinder;
    }

    void ProcessType(TypeDefinition type)
    {
        RemoveAttributes(type.CustomAttributes);
        foreach (var property in type.Properties)
        {
            RemoveAttributes(property.CustomAttributes);
        }
        foreach (var field in type.Fields)
        {
            RemoveAttributes(field.CustomAttributes);
        }
    }



    void RemoveAttributes(Collection<CustomAttribute> customAttributes)
    {
        var attributes = customAttributes
            .Where(attribute => propertyAttributeNames.Contains(attribute.Constructor.DeclaringType.FullName));

        foreach (var customAttribute in attributes.ToList())
        {
            customAttributes.Remove(customAttribute);
        }
    }

    public void Execute()
    {
        foreach (var type in allTypesFinder.AllTypes)
        {
            ProcessType(type);
        }
    }
}