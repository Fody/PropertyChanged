using System.ComponentModel;
using PropertyChanged;

public class ClassDependsOn : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}