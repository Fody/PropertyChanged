using System.ComponentModel;

public class ClassWithOnChangedCalculatedProperty :
    INotifyPropertyChanged
{
    public bool OnProperty2ChangedCalled;

    public string Property1 { get; set; }
    public string Property2 => $"{Property1}!";

    public void OnProperty2Changed()
    {
        OnProperty2ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}