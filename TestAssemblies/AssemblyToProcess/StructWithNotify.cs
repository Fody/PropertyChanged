using System.ComponentModel;

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
