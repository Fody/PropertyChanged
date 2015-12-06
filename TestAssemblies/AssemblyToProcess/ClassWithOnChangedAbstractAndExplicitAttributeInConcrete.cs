using System.ComponentModel;
using PropertyChanged;

public abstract class ClassWithOnChangedAbstractAndExplicitAttributeInConcrete : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }

    public abstract void OnChanged();

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedAbstractAndExplicitAttributeInConcreteConcrete : ClassWithOnChangedAbstractAndExplicitAttributeInConcrete
{
    [OnPropertyChanged("Property1")]
    public override void OnChanged()
    {
        OnProperty1ChangedCalled = true;
    }
}
