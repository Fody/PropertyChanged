using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitOnChangedAndNoPropertyChanged : INotifyPropertyChanged
{
    public int OnProperty1ChangedCalled;
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnChanged();
        }
    }
    [OnPropertyChanged("Property1")]
    public void OnChanged ()
    {
        OnProperty1ChangedCalled++;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}