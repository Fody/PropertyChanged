using System.Collections.Generic;
using Mono.Cecil;

public class PropertyData
{
    public FieldReference BackingFieldReference;
    public List<PropertyDefinition> AlsoNotifyFor = new List<PropertyDefinition>();
    public PropertyDefinition PropertyDefinition;
    public List<string> AlreadyNotifies = new List<string>();
}