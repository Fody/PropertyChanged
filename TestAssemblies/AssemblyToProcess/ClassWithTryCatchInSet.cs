using System;
using System.ComponentModel;

public class ClassWithTryCatchInSet:INotifyPropertyChanged
{
    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            try
            {
                property1 = value;
            }
            catch (ArgumentException)
            {
                // actually, 'call OnPropertyChanged' inserted here, it's wrong.
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}