using System;
using System.ComponentModel;

public class ClassSetterEndsOnThrowInRelease : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string item;
    public string Item
    {
        get { return item; }
        set
        {
            if (value == "Foo")
                item = "Bar";
            else
                throw new ArgumentOutOfRangeException("Item");
        }
    }
}