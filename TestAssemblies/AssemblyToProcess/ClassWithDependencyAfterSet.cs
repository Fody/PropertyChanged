using System.ComponentModel;

public class ClassWithDependencyAfterSet :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string property2;

// ReSharper disable ConvertToAutoProperty
    public string Property2
    {
        get => property2;
        set => property2 = value;
    }
// ReSharper restore ConvertToAutoProperty

    string property1;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            Property2 = value;
        }
    }
}