using System.ComponentModel;
using PropertyChanged;

public class ClassWithDependsOnAndDoNotNotify : INotifyPropertyChanged
{
    [DoNotNotify]
    public string UseLessProperty { get; set; }
    public string Property1 { get; set; }    
    public string Property2 => Property1;


    public event PropertyChangedEventHandler PropertyChanged;
}