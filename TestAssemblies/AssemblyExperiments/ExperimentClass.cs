using System.ComponentModel;
using PropertyChanged;

public class ExperimentClass : INotifyPropertyChanged
{

    [DoNotNotify]
    public string AAA { get; set; }
    public string Property1 { get; set; }
    public string Property2 { get { return Property1; } }


    public event PropertyChangedEventHandler PropertyChanged;
}
