using System.ComponentModel;

public class ClassWithNotifyInChildByInterface : ParentClass, INotifyPropertyChanged
{
    public string Property { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}