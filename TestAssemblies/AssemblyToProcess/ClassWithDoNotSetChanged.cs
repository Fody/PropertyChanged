using System.ComponentModel;
using PropertyChanged;

public class ClassWithDoNotSetChanged : INotifyPropertyChanged
{
    [DoNotSetChanged]
    public string Property1 { get; set; }
    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
