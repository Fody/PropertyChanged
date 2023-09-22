namespace AssemblyWithBase.StaticEqualsGenericParent;

public class BaseClass<T>
{
    static bool staticEqualsCalled;
    public bool StaticEqualsCalled
    {
        get => staticEqualsCalled;
        set => staticEqualsCalled = value;
    }

    public static bool Equals(BaseClass<T> first, BaseClass<T> second)
    {
        staticEqualsCalled = true;

        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first == null || second == null)
        {
            return false;
        }

        return first.Equals(second);
    }
}