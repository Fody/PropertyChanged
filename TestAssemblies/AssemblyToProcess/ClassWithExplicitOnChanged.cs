using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitOnChanged : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }

    [OnPropertyChanged("Property1")]
    public void OnChanged ()
    {
        OnProperty1ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}