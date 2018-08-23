using System.ComponentModel;

public class ClassWithOnChangedBeforeAfterWrongType : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public int Before;
    public int After;

    public string Property1 { get; set; }

    public void OnProperty1Changed(int before, int after)
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