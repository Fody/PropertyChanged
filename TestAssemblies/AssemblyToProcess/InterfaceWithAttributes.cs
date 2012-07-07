using PropertyChanged;

public interface InterfaceWithAttributes
{
    [DependsOn("a")]
    [DoNotNotify]
    string Property1 { get; set; }
}