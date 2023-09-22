namespace AssemblyWithBase.StaticEquals;

public class BaseClass
{
    static bool staticEqualsCalled;
    public bool StaticEqualsCalled
    {
        get => staticEqualsCalled;
        set => staticEqualsCalled = value;
    }

    public static bool Equals(BaseClass first, BaseClass second)
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