using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "NotAccessedField.Global")]
public class DependentPropertiesClassToTest :
    INotifyPropertyChanged
{
    public int OnProperty1ChangedCallCount;
    public int OnProperty2ChangedCallCount;

    public string Property1 { get; set; }
    public string Property2 => $"{Property1}";

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        switch(propertyName)
        {
            case nameof(Property1):
                ++OnProperty1ChangedCallCount;
                break;
            case nameof(Property2):
                ++OnProperty2ChangedCallCount;
                break;
        }

        PropertyChanged?.Invoke(this, new(propertyName));
    }
}