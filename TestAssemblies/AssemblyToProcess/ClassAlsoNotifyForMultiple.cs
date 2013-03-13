using System.ComponentModel;
using PropertyChanged;

public class ClassAlsoNotifyForMultiple : INotifyPropertyChanged
{
    [AlsoNotifyFor("Property2", "Property3")]
    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public string Property3 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}