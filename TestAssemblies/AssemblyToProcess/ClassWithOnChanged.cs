using System.ComponentModel;

public class ClassWithOnChanged : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }
    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}