using Mono.Cecil;

public class EqualityComparerRef
{
    public EqualityComparerRef(MethodReference defaultRef, MethodReference equalsRef)
    {
        DefaultRef = defaultRef;
        EqualsRef = equalsRef;
    }

    public MethodReference DefaultRef { get; }
    public MethodReference EqualsRef { get; }
}
