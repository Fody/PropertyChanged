using System.ComponentModel;

public class ClassWithIndexerReferencingPropertyAndBeforeAfter :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public string this[int i]
    {
        get => Property1;
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
