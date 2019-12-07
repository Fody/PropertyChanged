using System.ComponentModel;
using PropertyChanged;

public class ClassWithOnChangedCustomized : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;
    public bool FirstCustomCalled;
    public bool SecondCustomCalled;
    public int PropertyChangedCounterValue;

    string property2;

    [OnChangedMethod(nameof(FirstCustom))]
    [OnChangedMethod(nameof(SecondCustom))]
    public string Property1 { get; set; }

    [OnChangedMethod(nameof(IncrementPropertyChangedCounter))]
    public string Property2
    {
        get => property2;
        set
        {
            property2 = value;
            IncrementPropertyChangedCounter();
        }
    }

    [OnChangedMethod(nameof(IncrementPropertyChangedCounter))]
    [OnChangedMethod(nameof(IncrementPropertyChangedCounter))]
    [OnChangedMethod(nameof(IncrementPropertyChangedCounter))]
    public string Property3 { get; set; }

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

    void IncrementPropertyChangedCounter()
    {
        ++PropertyChangedCounterValue;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
