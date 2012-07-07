using System.ComponentModel;

public class ClassThatIsNotSealed : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}