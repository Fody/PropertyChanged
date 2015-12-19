using System.ComponentModel;
using PropertyChanged;

public class ClassWithBeforeAfterValueCheckImplementation : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    [DoNotNotify]
    public string BeforeValue1 { get; set; }

    [DependsOn("Property1")]
    public string Property2 => Property1 + "2";

    [DoNotNotify]
    public string BeforeValue2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        if (propertyName.Equals("Property1"))
        {
            BeforeValue1 = (string)before;
        }

        if (propertyName.Equals("Property2"))
        {
            BeforeValue2 = (string)before;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}