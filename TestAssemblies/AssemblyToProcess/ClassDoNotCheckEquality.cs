using System;
using System.ComponentModel;

public class ClassDoNotCheckEquality : INotifyPropertyChanged
{
    public int timesProperty1Changed = 0;
    public int timesProperty2Changed = 0;
    
    public string Property1 { get; set; }
    
    [PropertyChanged.DoNotCheckEquality]
    public string Property2 { get; set; }

    public void OnProperty1Changed() 
    {
        timesProperty1Changed += 1;
    }

    public void OnProperty2Changed()
    {
        timesProperty2Changed += 1;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}