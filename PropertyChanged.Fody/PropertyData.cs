using System.Collections.Generic;
using Mono.Cecil;

public class PropertyData
{

    public PropertyData()
    {
        AlsoNotifyFor = new List<PropertyDefinition>();
    }

    public FieldReference BackingFieldReference;
    public List<PropertyDefinition> AlsoNotifyFor;
    public PropertyDefinition PropertyDefinition;
}