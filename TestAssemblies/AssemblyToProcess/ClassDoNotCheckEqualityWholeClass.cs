using System.ComponentModel;

[PropertyChanged.DoNotCheckEquality]
public class ClassDoNotCheckEqualityWholeClass : INotifyPropertyChanged
{
    public int TimesProperty1Changed = 0;
    public int TimesProperty2Changed = 0;

    public string Property1 { get; set; }
    
    public string Property2 { get; set; }

    public void OnProperty1Changed()
    {
        TimesProperty1Changed += 1;
    }

    public void OnProperty2Changed()
    {
        TimesProperty2Changed += 1;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}