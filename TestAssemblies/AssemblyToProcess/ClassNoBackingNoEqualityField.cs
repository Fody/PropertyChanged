using System.ComponentModel;

public class ClassNoBackingNoEqualityField : INotifyPropertyChanged
{

    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}