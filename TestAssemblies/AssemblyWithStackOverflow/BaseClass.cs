    using System.ComponentModel;

public class BaseClass : INotifyPropertyChanged
{
    public virtual string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName, object before, object after)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}