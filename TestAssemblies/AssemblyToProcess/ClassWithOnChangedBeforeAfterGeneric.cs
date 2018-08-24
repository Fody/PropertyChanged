using System.ComponentModel;

public class ClassWithOnChangedBeforeAfterGeneric : INotifyPropertyChanged
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

    public void OnPropertyChanged<T>(string propertyName, T before, T after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}


public class ClassWithOnChangedBeforeAfterGeneric<T> : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public T Before;
    public T After;

    public T Property1 { get; set; }

    public void OnProperty1Changed(T before, T after)
    {
        OnProperty1ChangedCalled = true;
        Before = before;
        After = after;
    }

    public void OnPropertyChanged(string propertyName, T before, T after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}