using PropertyChanged;

public struct StructWithAttributes
{
    [DependsOn("a")]
    [DoNotNotify]
    public string Property1 { get; set; }
}