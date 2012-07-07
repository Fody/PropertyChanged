using System.ComponentModel;

public class ClassParentWithProperty : INotifyPropertyChanged
{
    public string Property2 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}