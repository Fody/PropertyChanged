using System.ComponentModel;

public class ClassWithIsChanged : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;


}