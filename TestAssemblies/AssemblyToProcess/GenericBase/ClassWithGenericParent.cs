using System.ComponentModel;

public class ClassWithGenericParent<T> : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}