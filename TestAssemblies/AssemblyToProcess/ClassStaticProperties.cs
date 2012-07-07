using System.ComponentModel;

public class ClassStaticProperties : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public static string Property { get; set; }
}