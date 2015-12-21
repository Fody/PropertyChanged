using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    IEnumerable<PropertyDefinition> GetOnPropertyChangedAttributeProperties(CustomAttribute onPropertyChangedAttribute, MethodDefinition method, List<PropertyDefinition> allProperties)
    {
        var customAttributeArguments = onPropertyChangedAttribute.ConstructorArguments.ToList();
        var value = (string)customAttributeArguments[0].Value;
        yield return GetOnPropertyChangedAttributeProperty(method, allProperties, value);

        if (customAttributeArguments.Count > 1)
        {
            var paramsArguments = (CustomAttributeArgument[])customAttributeArguments[1].Value;
            foreach (string argument in paramsArguments.Select(x => x.Value))
            {
                yield return GetOnPropertyChangedAttributeProperty(method, allProperties, argument);
            }
        }
    }

    static PropertyDefinition GetOnPropertyChangedAttributeProperty(MethodDefinition method, List<PropertyDefinition> allProperties, string argument)
    {
        var propertyDefinition = allProperties.FirstOrDefault(x => x.Name == argument);
        if (propertyDefinition == null)
        {
            throw new WeavingException(string.Format("Could not find property '{0}' for OnPropertyChanged attribute assigned to '{1}'.", argument, method.Name));
        }
        return propertyDefinition;
    }
}