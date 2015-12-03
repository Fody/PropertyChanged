using Mono.Cecil;
using Mono.Collections.Generic;

public enum OnChangedTypes
{
    NoArg,
    BeforeAfter,
}

public class OnChangedMethod
{
    public MethodReference MethodReference;
    public OnChangedTypes OnChangedType;
}

public class ExplicitOnChangedMethod
{
    public MethodReference MethodReference;
    public Collection<CustomAttribute> CustomAttributes;
    public OnChangedTypes OnChangedType;
}