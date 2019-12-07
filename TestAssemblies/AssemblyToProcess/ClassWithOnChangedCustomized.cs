using System.ComponentModel;
using PropertyChanged;

public class ClassWithOnChangedCustomized : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public bool FirstCustomCalled;
    public bool SecondCustomCalled;

    [OnChangedMethod(nameof(FirstCustom))]
    [OnChangedMethod(nameof(SecondCustom))]
    public string Property1 { get; set; }

    public void OnProperty1Changed()
    {
        OnProperty1ChangedCalled = true;
    }

    void FirstCustom()
    {
        FirstCustomCalled = true;
    }

    void SecondCustom()
    {
        SecondCustomCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
