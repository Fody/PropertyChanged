using System.ComponentModel;

public class ClassAlreadyHasSingleNotification :
    INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            OnPropertyChanged("Property1");
        }
    }
    public string Property2 => Property1;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}