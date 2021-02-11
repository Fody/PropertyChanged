using System.Collections.Generic;
using Mono.Cecil;

public class PropertyData
{
    public TypeNode ParentType;
    public FieldReference BackingFieldReference;
    public List<PropertyDefinition> AlsoNotifyFor = new List<PropertyDefinition>();
    public PropertyDefinition PropertyDefinition;
    public MethodReference EqualsMethod;
    public List<string> AlreadyNotifies = new List<string>();
}
