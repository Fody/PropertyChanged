using System.ComponentModel;

public class ClassMissingSetGet : INotifyPropertyChanged
{
    string property;

    public string PropertyNoSet
    {
        get { return property; }
    }


    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

}