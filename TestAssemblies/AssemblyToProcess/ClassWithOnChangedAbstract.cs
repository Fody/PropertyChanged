using System.ComponentModel;

public abstract class ClassWithOnChangedAbstract : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    public string Property1 { get; set; }
    public abstract void OnProperty1Changed();

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedConcrete : ClassWithOnChangedAbstract
{
    public override void OnProperty1Changed()
    {
        OnProperty1ChangedCalled = true;
    }
}
