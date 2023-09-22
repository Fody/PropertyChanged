using System.ComponentModel;

public class ClassAlreadyHasSingleNotificationDiffParamLocation :
    INotifyPropertyChanged
{
    string property1;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            OnPropertyChanged(7, "Property1");
        }
    }
    public string Property2 => Property1;

    public virtual void OnPropertyChanged(int fake, string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}