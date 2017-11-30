using System.ComponentModel;
// ReSharper disable ValueParameterNotUsed

public class ClassNoBackingWithEqualityField : INotifyPropertyChanged
{
    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}