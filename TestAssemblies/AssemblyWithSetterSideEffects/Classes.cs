using System.ComponentModel;

public class WithSideEffectBeforeValueAssignment :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    string _property1;
    public string Property1
    {
        get => _property1;
        set
        {
            CallSideEffectBefore();
            _property1 = value;
        }
    }

    
    public int SideEffectBeforeCallCount { get; set; }
    void CallSideEffectBefore() => SideEffectBeforeCallCount++;
}

public class WithSideEffectAfterValueAssignment :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }



    string _property1;
    public string Property1
    {
        get => _property1;
        set
        {
            _property1 = value;
            CallSideEffectAfter();
        }
    }

    public int SideEffectAfterCallCount { get; set; }
    void CallSideEffectAfter() => SideEffectAfterCallCount++;
}