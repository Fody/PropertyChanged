using System.ComponentModel;

public class ClassWithGenericMiddleBase : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
