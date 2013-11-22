using Mono.Cecil;

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