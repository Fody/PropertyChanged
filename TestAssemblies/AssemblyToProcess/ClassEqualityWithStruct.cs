using System.ComponentModel;

public class ClassEqualityWithStruct : INotifyPropertyChanged
{
    public SimpleStruct Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    
    public struct SimpleStruct
    {
    }

}