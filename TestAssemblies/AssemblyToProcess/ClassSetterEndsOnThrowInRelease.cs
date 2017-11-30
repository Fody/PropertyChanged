using System;
using System.ComponentModel;

public class ClassSetterEndsOnThrowInRelease : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string item;
    public string Item    {
        get => item;        set
        {
            if (value == "Foo")
            {
                item = "Bar";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}