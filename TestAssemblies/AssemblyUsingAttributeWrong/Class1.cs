using System.ComponentModel;
using PropertyChanged;
// ReSharper disable UnusedVariable


public class Class1 : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public Class1()
    {
        var type = typeof(DependsOnAttribute);
    }
}