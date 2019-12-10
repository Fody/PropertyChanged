using System.ComponentModel;

public class ClassWithFieldGetButNoFieldSet :
    INotifyPropertyChanged
{
    string property;

    public string Property1
    {
        get => property;
        set => SetField(value);
    }

    void SetField(string value)
    {
        property = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}