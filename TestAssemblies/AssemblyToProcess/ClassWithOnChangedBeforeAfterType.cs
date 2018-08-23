using System.ComponentModel;

public class ClassWithOnChangedBeforeAfterType : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public string Before;
    public string After;

    public string Property1 { get; set; }

    public void OnProperty1Changed(string before, string after)
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