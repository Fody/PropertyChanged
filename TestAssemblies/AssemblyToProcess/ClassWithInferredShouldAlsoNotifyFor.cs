using System.ComponentModel;

public class ClassWithInferredShouldAlsoNotifyFor :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public string Property2 => Property1;
    public event PropertyChangedEventHandler PropertyChanged;
}