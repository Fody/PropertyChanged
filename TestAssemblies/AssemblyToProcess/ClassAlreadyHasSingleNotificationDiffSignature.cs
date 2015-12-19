using System.ComponentModel;

public class ClassAlreadyHasSingleNotificationDiffSignature : INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanged("Property1",5);
        }
    }

    public virtual void OnPropertyChanged(string propertyName, int fake)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}