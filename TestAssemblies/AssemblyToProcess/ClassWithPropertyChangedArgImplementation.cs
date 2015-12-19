using System.ComponentModel;
using PropertyChanged;

public class ClassWithPropertyChangedArgImplementation : INotifyPropertyChanged
{

    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(PropertyChangedEventArgs arg)
    {
        PropertyChanged?.Invoke(this, arg);
    }

}