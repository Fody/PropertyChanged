using System.ComponentModel;

public class ClassEquality : INotifyPropertyChanged
{
    public string StringProperty { get; set; }
    public int IntProperty { get; set; }
    public int? NullableIntProperty { get; set; }
    public bool BoolProperty { get; set; }
    public bool? NullableBoolProperty { get; set; }
    public object ObjectProperty { get; set; }
    public string[] ArrayProperty { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}