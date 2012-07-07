public class ChildClass : BaseClass
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
