    using AssemblyWithBase.Simple;

public class ChildWithBaseInDifferentAssembly : BaseClassWithVirtualProperty
{
    public override string Property1
    {
        get
        {
            if (string.IsNullOrEmpty(base.Property1))
            {
                base.Property1 = "test";
            }

            return base.Property1;
        }

        set { base.Property1 = value; }
    }
}
