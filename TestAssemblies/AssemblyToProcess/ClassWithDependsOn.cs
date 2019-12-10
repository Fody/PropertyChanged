using System.ComponentModel;
using PropertyChanged;

public class ClassWithDependsOn :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}