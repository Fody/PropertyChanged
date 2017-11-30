using System.ComponentModel;

public class ClassWithBoolPropUsingStringProp: INotifyPropertyChanged
{
    public bool StringCompareProperty => StringProperty == "magicString";

    public bool BoolProperty { get; set; }

    string stringProperty;
    public string StringProperty    {
        get => stringProperty;        set
        {
            stringProperty = value;
            if (StringCompareProperty)
            {
                BoolProperty = true;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}