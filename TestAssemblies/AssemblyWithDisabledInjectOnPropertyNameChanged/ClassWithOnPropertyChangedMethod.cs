using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "NotAccessedField.Global")]
public class ClassWithOnPropertyChangedMethod : INotifyPropertyChanged
{
    public int OnProperty1ChangedCallCount;
    
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    void OnProperty1Changed()
    {
        ++OnProperty1ChangedCallCount;
    }
}
