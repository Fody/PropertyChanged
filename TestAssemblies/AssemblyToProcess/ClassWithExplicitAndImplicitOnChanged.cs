using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitAndImplicitOnChanged : INotifyPropertyChanged
{
    public bool ImplicitMethodCalled;
    public bool ExplicitMethodCalled;

    public string Property1 { get; set; }

    public void OnProperty1Changed()
    {
        ImplicitMethodCalled = true;
    }

    [OnPropertyChanged("Property1")]
    public void OnChanged ()
    {
        ExplicitMethodCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}