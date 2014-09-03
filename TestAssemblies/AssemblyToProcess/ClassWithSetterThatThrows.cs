using System;
using System.ComponentModel;

public class ClassWithSetterThatThrows : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Item
    {
        get { return "Foo"; }
        set
        {
            throw new NotSupportedException("Obsolete Setter"); 
        }
    }
}