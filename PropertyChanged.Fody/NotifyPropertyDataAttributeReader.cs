using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public NotifyPropertyData ReadAlsoNotifyForData(PropertyDefinition property, List<PropertyDefinition> allProperties)
    {
        var notifyAttribute = property.CustomAttributes.GetAttribute("PropertyChanged.AlsoNotifyForAttribute");
        if (notifyAttribute == null)
        {
            return null;
        }
        var propertyNamesToNotify = GetPropertyNamesToNotify(notifyAttribute, property, allProperties);

        return new NotifyPropertyData
                   {
                       AlsoNotifyFor = propertyNamesToNotify.ToList(),
                   };
    }

    IEnumerable<PropertyDefinition> GetPropertyNamesToNotify(CustomAttribute notifyAttribute, PropertyDefinition property, List<PropertyDefinition> allProperties)
    {
        var customAttributeArguments = notifyAttribute.ConstructorArguments.ToList();
        var value = (string)customAttributeArguments[0].Value;
        yield return GetPropertyDefinition(property, allProperties, value);
        if (customAttributeArguments.Count > 1)
        {
            foreach (string argument in customAttributeArguments.Select(x => x.Value))
            {
                yield return GetPropertyDefinition(property, allProperties, argument);
            }
        }
    }

    static PropertyDefinition GetPropertyDefinition(PropertyDefinition property, List<PropertyDefinition> allProperties, string argument)
    {
        var propertyDefinition = allProperties.FirstOrDefault(x => x.Name == argument);
        if (propertyDefinition == null)
        {
            throw new WeavingException(string.Format("Could not find property '{0}' for AlsoNotifyFor attribute assigned to '{1}'.", argument, property.Name));
        }
        return propertyDefinition;
    }
}