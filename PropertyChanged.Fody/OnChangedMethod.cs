using System.Collections.Generic;
using Mono.Cecil;

public enum OnChangedTypes
{
    None,
    NoArg,
    BeforeAfter,
    BeforeAfterTyped
}

public class OnChangedMethod
{
    public MethodReference MethodReference;
    public MethodDefinition MethodDefinition;
    public OnChangedTypes OnChangedType;
    public string ArgumentTypeFullName;
    public bool IsDefaultMethod;
    public List<PropertyDefinition> Properties = new();
}


