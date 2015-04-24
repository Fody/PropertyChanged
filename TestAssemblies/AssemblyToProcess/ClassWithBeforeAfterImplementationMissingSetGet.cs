using System.ComponentModel;
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

public class ClassWithBeforeAfterImplementationMissingSetGet : INotifyPropertyChanged
{
    string property;

    public string PropertyNoSet
    {
        get { return property; }
    }


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}