using System.ComponentModel;

public class ClassToTest : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

}
public class GenericClassToTest<T> : INotifyPropertyChanged
{
    public T Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

}
