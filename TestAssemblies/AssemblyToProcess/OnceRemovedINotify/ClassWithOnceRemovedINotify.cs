using System.ComponentModel;

public class ClassWithOnceRemovedINotify : INotifyPropertyChangedChild
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}