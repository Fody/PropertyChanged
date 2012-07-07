using System;
using System.ComponentModel;

public class ClassWithExceptionInProperty : INotifyPropertyChanged
{
    public string Property
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}