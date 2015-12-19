using System.ComponentModel;
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

public class ClassWithBeforeAfterImplementationMissingSetGet : INotifyPropertyChanged
{
    string property;

    public string PropertyNoSet => property;


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}