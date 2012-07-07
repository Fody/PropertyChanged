using System.ComponentModel;

public abstract class ClassWithAbstractProperty : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public abstract string Property1 { get; set; }
}