using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitOnChangedAndOnPropertyChanged : INotifyPropertyChanged
{
    public int OnProperty1ChangedCalled;
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanged("Property1");
            OnChanged();
        }
    }

    [OnPropertyChanged("Property1")]
    public void OnChanged ()
    {
        OnProperty1ChangedCalled++;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}