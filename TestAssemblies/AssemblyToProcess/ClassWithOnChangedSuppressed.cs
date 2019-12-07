using System.ComponentModel;
using PropertyChanged;

public class ClassWithOnChangedSuppressed : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public bool OnProperty2ChangedCalled;

    [OnChangedMethod(null)]
    public string Property1 { get; set; }

    [OnChangedMethod("")]
    public string Property2 { get; set; }

    public void OnProperty1Changed()
    {
        OnProperty1ChangedCalled = true;
    }

    public void OnProperty2Changed()
    {
        OnProperty2ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
