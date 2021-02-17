using System.Diagnostics;
using PropertyChanged;


[AddINotifyPropertyChangedInterface]
public class ClassWithWarningsBase
{
    public string BaseClassProperty { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class ClassWithWarnings : ClassWithWarningsBase
{

    [DoNotNotify]
    public string Property1 { get; set; }

    [OnChangedMethod(nameof(CallThisInstead))]
    public string Property2 { get; set; }

    public bool Property1ChangedCalled;
    public bool Property2ChangedCalled;
    public bool PropertyXChangedCalled;
    public bool CallThisInsteadCalled;


    void OnProperty1Changed() => Trace.WriteLine("Ignored");

    void OnProperty2Changed() => Trace.WriteLine("Ignored");

    void OnPropertyXChanged() => Trace.WriteLine("Ignored");
    void OnBaseClassPropertyChanged() => Trace.WriteLine("Ignored");

    void CallThisInstead() => Trace.WriteLine("Ignored");
}