using System.ComponentModel;

public class ClassWithNonVoidOnPropertyChanged : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public int OnProperty1Changed()
    {
        return 0;
    }
}