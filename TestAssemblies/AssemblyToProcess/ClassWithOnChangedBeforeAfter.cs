using System.ComponentModel;

public class ClassWithOnChangedBeforeAfter : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public bool OnProperty2ChangedCalled;

    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }

    public void OnProperty2Changed(object before, object after)
    {
        OnProperty2ChangedCalled = true;
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}