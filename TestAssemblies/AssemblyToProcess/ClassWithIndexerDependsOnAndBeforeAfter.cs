using System.ComponentModel;
using PropertyChanged;

public class ClassWithIndexerDependsOnAndBeforeAfter :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }

    [DependsOn(nameof(Property1))]
    public string this[int i]
    {
        get => null;
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
