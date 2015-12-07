using System.ComponentModel;
using PropertyChanged;

public class ClassWithExplicitOnChangedForMultipleProperties : INotifyPropertyChanged
{
    public int OnChangedCalled;

    public string Property1 { get; set; }
    public string Property2 { get; set; }

    [OnPropertyChanged("Property1", "Property2")]
    public void OnChanged ()
    {
        OnChangedCalled++;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}