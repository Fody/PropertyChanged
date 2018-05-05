using ReactiveUI;

public class ClassReactiveUI : ReactiveObject
{
    public string Property1 { get; set; }
}

public class ClassReactiveUI2 : ClassReactiveUI
{
    public string Property2 { get; set; }
}
