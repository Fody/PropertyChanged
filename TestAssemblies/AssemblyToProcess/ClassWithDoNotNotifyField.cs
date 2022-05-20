using System.ComponentModel;
using PropertyChanged;

public class ClassWithDoNotNotifyField :
    INotifyPropertyChanged
{
    [field: DoNotNotify]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}