using System.ComponentModel;
using PropertyChanged;

public class ClassWithDependsOnAndPropertyChanged :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public int Property2ChangedCalled = 0;

    private void OnProperty2Changed() => Property2ChangedCalled++;


    public event PropertyChangedEventHandler PropertyChanged;
}