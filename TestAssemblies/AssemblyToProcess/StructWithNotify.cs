using System.ComponentModel;
using PropertyChanged;

public struct StructWithNotify : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

public struct StructWithNotify<T> : INotifyPropertyChanged
{
    public T Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

[AddINotifyPropertyChangedInterface]
public struct StructWithNotifyAttribute
{
    public string Property1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public struct StructWithNotifyAttribute<T>
{
    public T Property1 { get; set; }
}
