using System.ComponentModel;
using PropertyChanged;


public class Class1 : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public Class1()
    {
#pragma warning disable 168
        var type = typeof(DependsOnAttribute);
#pragma warning restore 168
    }
}