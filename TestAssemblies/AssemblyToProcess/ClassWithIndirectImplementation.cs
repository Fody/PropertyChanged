using System.ComponentModel;

public class ClassWithIndirectImplementation :
    Indirect
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

public interface Indirect :
    INotifyPropertyChanged;