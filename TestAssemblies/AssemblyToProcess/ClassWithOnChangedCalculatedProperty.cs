using System.ComponentModel;

public class ClassWithOnChangedCalculatedProperty :
    INotifyPropertyChanged
{
    public bool OnProperty2ChangedCalled;
    public bool OnProperty3ChangedCalled;

    public string Property1 { get; set; }
    public string Property2 => $"{Property1}!";
    public string Property3 => $"{Property2}!";

    public void OnProperty2Changed()
    {
        OnProperty2ChangedCalled = true;
    }

    public void OnProperty3Changed()
    {
        OnProperty3ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}