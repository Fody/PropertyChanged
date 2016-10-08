using System.ComponentModel;

public class ClassWithOnChangedBeforeAfterGeneric : INotifyPropertyChanged
{
    public bool GenericOnPropertyChangedCalled;

    public string Property1 { get; set; }

    public void OnPropertyChanged<T>(string propertyName, T before, T after)
    {
        GenericOnPropertyChangedCalled = true;
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}