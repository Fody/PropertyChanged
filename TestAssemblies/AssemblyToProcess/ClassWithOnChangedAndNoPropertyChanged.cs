using System.ComponentModel;

public class ClassWithOnChangedAndNoPropertyChanged : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    string property1;

    public string Property1    {
        get => property1;        set
        {
            property1 = value;
            OnProperty1Changed();
        }
    }

    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}