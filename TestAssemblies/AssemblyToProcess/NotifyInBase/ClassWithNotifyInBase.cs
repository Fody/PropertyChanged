using PropertyChanged;

public class ClassWithNotifyInBase : ClassParentWithProperty
{
    [AlsoNotifyFor("Property2")]
    public string Property1 { get; set; }
}