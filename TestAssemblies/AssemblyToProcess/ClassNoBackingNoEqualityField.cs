using System.ComponentModel;
// ReSharper disable ValueParameterNotUsed

public class ClassNoBackingNoEqualityField : INotifyPropertyChanged
{
    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}