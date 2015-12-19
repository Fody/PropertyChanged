    using System.ComponentModel;

public class BaseClass : INotifyPropertyChanged
{
    public virtual string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}