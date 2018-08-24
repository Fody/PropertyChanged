using System.ComponentModel;

public class ClassWithBeforeAfterGeneric : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public bool GenericOnPropertyChangedCalled;
    public object Before;
    public object After;

    protected virtual void OnPropertyChanged<T>(string propertyName, T before, T after)
    {
        GenericOnPropertyChangedCalled = true;
        Before = before;
        After = after;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithBeforeAfterGeneric<T> : INotifyPropertyChanged
{
    public T Property1 { get; set; }
    public bool GenericOnPropertyChangedCalled;
    public T Before;
    public T After;

    protected virtual void OnPropertyChanged(string propertyName, T before, T after)
    {
        GenericOnPropertyChangedCalled = true;
        Before = before;
        After = after;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}