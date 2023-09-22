namespace AssemblyWithBaseInDifferentModule.Hierarchy;

using AssemblyWithBase.StaticEquals;

public class ChildClass : StaticEquals
{
    string property1;
    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            Property2 = new();
        }
    }

    public BaseClass Property2 { get; set; }
}