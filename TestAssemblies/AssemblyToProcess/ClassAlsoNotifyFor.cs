using System.ComponentModel;
using PropertyChanged;

public class ClassAlsoNotifyFor : INotifyPropertyChanged
{
    [AlsoNotifyFor("Property2")]
    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}