using System.ComponentModel;

public class ClassWithInvalidIsChangedType : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public string IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}