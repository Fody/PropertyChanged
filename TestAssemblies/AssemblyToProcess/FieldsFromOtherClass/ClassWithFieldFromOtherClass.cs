using System.ComponentModel;

public class ClassWithFieldFromOtherClass :
    INotifyPropertyChanged
{
    OtherClass otherClass = new();

    public string Property1
    {
        get => otherClass.property1;
        set => otherClass.property1 = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}