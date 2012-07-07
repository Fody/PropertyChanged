using System.ComponentModel;

public class ClassWithFieldFromOtherClass : INotifyPropertyChanged
{
    OtherClass otherClass;

    public ClassWithFieldFromOtherClass()
    {
        otherClass = new OtherClass();
    }

    public string Property1
    {
        get { return otherClass.property1; }
        set { otherClass.property1 = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}