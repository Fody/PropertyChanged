using System.ComponentModel;
using PropertyChanged;

public class ClassWithBeforeAfterAndSimpleImplementation : INotifyPropertyChanged
{

    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {

    }
    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}