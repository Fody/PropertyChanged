using System.ComponentModel;
using PropertyChanged;

[DoNotNotify]
public class ClassWithDoNotNotify : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
}