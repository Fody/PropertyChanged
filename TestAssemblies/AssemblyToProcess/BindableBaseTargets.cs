using System.ComponentModel;

public class ClassBindableBaseCallingOnPropertyChanged : BindableBase
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            OnPropertyChanged("Property2");
        }
    }
}
public class ClassBindableBaseCallingSetProperty : BindableBase
{
    string property1;

    public string Property1
    {
        get { return property1; }
        set
        {
            SetProperty(ref property1, value, "Property1");
        }
    }
}
public abstract class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName = null)
    {
        if (Equals(storage, value)) return false;

        storage = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
