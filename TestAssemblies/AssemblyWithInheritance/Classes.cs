using System.Collections.Generic;
using System.ComponentModel;

public class BaseClass : INotifyPropertyChanged
{
    public IList<string> ChangedEvents = new List<string>();

    public virtual int Property1 { get; set; }
    public int Property2 => Property1 + 1;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        ChangedEvents.Add(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DerivedClass : BaseClass
{
    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value;
    }

    public int Property3 => Property1 - 1;
}