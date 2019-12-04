using System.ComponentModel;

public class ClassWithStaticOnPropertyChanged : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public static void OnProperty1Changed()
    {
    }
}
