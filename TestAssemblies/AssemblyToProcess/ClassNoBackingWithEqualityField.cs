using System.ComponentModel;

public class ClassNoBackingWithEqualityField : INotifyPropertyChanged
{

    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
