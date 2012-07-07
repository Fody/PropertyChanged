using System;
using System.ComponentModel;

public class ClassWithPropertySetInCatch:INotifyPropertyChanged
{
    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            try
            {
                throw new ArgumentException();
            }
            catch (ArgumentException)
            {
                property1 = value;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}