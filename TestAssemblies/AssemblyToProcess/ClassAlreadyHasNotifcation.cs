using System.ComponentModel;

public class ClassAlreadyHasNotifcation : INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            if (property1 != value)
            {
                property1 = value;
                OnPropertyChanged("Property1");
            }
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