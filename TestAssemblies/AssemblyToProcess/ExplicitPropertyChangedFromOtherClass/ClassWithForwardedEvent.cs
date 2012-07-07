using System.ComponentModel;

public class ClassWithForwardedEvent : INotifyPropertyChanged
{
    InnerClass inner;

    public event PropertyChangedEventHandler PropertyChanged
    {
        add { inner.PropertyChanged += value; }
        remove { inner.PropertyChanged -= value; }
    }

}
