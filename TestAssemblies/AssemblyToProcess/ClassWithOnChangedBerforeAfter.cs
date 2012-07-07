using System.ComponentModel;

public class ClassWithOnChangedBerforeAfter : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }
    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}