using System.ComponentModel;

public class ClassWithPublicField : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public bool Property1;
}