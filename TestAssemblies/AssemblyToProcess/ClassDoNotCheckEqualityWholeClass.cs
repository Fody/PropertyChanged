using System.ComponentModel;

[PropertyChanged.DoNotCheckEquality]
public class ClassDoNotCheckEqualityWholeClass : INotifyPropertyChanged
{
    public int timesProperty1Changed = 0;
    public int timesProperty2Changed = 0;

    public string Property1 { get; set; }
    
    public string Property2 { get; set; }

    public void OnProperty1Changed()
    {
        this.timesProperty1Changed += 1;
    }

    public void OnProperty2Changed()
    {
        this.timesProperty2Changed += 1;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}