using System.ComponentModel;

public class ClassDoNotCheckEquality : INotifyPropertyChanged
{
    public int TimesProperty1Changed;
    public int TimesProperty2Changed;

    public string Property1 { get; set; }

    [PropertyChanged.DoNotCheckEquality]
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