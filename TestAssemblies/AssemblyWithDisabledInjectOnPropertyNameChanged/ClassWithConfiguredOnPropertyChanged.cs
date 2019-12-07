using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PropertyChanged;

[SuppressMessage("ReSharper", "NotAccessedField.Global")]
public class ClassWithConfiguredOnPropertyChanged : INotifyPropertyChanged
{
    public int OnProperty1ChangedCallCount;
    
    [OnChangedMethod(nameof(OnProperty1Changed))]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    void OnProperty1Changed()
    {
        ++OnProperty1ChangedCallCount;
    }
}
