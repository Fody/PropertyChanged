using System.ComponentModel;

public class ClassWithOnChangedAndNoPropertyChanged : INotifyPropertyChanged
{
    public int OnProperty1ChangedCalled;
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnProperty1Changed();
        }
    }

    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled++;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}