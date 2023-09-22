using System.Collections.Generic;
using Mono.Cecil;

public class PropertyData
{
    public TypeNode ParentType;
    public FieldReference BackingFieldReference;
    public List<PropertyDefinition> AlsoNotifyFor = new();
    public PropertyDefinition PropertyDefinition;
    public MethodReference EqualsMethod;
    public List<string> AlreadyNotifies = new();
}
