using System.ComponentModel;

public class ClassWithOnChangedAndOnPropertyChanged : INotifyPropertyChanged
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
            OnProperty1Changed();
        }
    }

    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled++;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}