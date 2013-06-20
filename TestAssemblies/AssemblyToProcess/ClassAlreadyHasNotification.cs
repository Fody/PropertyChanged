using System.ComponentModel;

public class ClassAlreadyHasNotification : INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanged("Property1");
            OnPropertyChanged("Property2");
        }
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}