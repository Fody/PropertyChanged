using System.ComponentModel;

public sealed class ClassThatIsSealed : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}