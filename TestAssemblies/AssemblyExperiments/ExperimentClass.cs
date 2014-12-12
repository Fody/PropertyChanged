using System.ComponentModel;

public class ClassExperiment:Base
{
    public bool MyProperty { get; set; }
}

public class Base:INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var eventHandler = PropertyChanged;
        if (eventHandler != null)
        {
            eventHandler(sender, e);
        }
    }
}

