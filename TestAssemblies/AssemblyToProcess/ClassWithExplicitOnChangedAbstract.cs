using System.ComponentModel;
using PropertyChanged;

public abstract class ClassWithExplicitOnChangedAbstract : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }
    [OnPropertyChanged("Property1")]
    public abstract void OnChanged();

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithExplicitOnChangedConcrete : ClassWithExplicitOnChangedAbstract
{
    public override void OnChanged()
    {
        OnProperty1ChangedCalled = true;
    }
}
