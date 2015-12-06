using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitOnChangedBeforeAfter : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public bool OnProperty2ChangedCalled;

    public string Property1 { get; set; }
    public string Property2 { get; set; }

    [OnPropertyChanged("Property1")]
    public void On1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }

    [OnPropertyChanged("Property2")]
    public void On2Changed(object before, object after)
    {
        OnProperty2ChangedCalled = true;
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}