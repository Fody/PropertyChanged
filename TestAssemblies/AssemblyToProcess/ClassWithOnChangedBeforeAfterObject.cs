using System.ComponentModel;

public class ClassWithOnChangedBeforeAfterObject : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public object Before;
    public object After;

    public string Property1 { get; set; }

    public void OnProperty1Changed(object before, object after)
    {
        OnProperty1ChangedCalled = true;
        Before = before;
        After = after;
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}