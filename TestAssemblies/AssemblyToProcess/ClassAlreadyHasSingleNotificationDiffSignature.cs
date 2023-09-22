using System.ComponentModel;

public class ClassAlreadyHasSingleNotificationDiffSignature :
    INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            OnPropertyChanged("Property1", 5);
        }
    }

    public virtual void OnPropertyChanged(string propertyName, int fake)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}