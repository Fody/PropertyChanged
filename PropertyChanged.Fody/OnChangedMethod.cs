using Mono.Cecil;

public enum OnChangedTypes
{
    NoArg,
    BeforeAfterObject,
    BeforeAfterType
}

public class OnChangedMethod
{
    public MethodReference MethodReference;
    public OnChangedTypes OnChangedType;
}