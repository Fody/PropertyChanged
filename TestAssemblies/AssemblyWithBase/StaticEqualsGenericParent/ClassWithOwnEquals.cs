namespace AssemblyWithBase.StaticEqualsGenericParent;

public class ClassWithOwnEquals : BaseClass<int>
{
    static bool childStaticEqualsCalled;

    public bool ChildStaticEqualsCalled
    {
        get => childStaticEqualsCalled;
        set => childStaticEqualsCalled = value;
    }

    public static bool Equals(ClassWithOwnEquals first, ClassWithOwnEquals second)
    {
        childStaticEqualsCalled = true;

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