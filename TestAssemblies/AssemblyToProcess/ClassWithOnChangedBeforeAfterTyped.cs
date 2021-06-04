using System.ComponentModel;

using PropertyChanged;

public class ClassWithOnChangedBeforeAfterTyped :
    INotifyPropertyChanged
{
    public string OnProperty1ChangedCalled;
    public string OnProperty2ChangedCalled;

    public string Property1 { get; set; }
    public int Property2 { get; set; }

    public void OnProperty1Changed(string before, string after)
    {
        OnProperty1ChangedCalled = before + "-" + after;
    }

    public void OnProperty2Changed(int before, int after)
    {
        OnProperty2ChangedCalled = before + "-" + after;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedBeforeAfterTypedInvalidSignatureDefault :
    INotifyPropertyChanged
{
    public string OnProperty1ChangedCalled;

    public string Property1 { get; set; }

    public void OnProperty1Changed(int before, int after)
    {
        OnProperty1ChangedCalled = before + "-" + after;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedBeforeAfterTypedInvalidSignatureExplicit :
    INotifyPropertyChanged
{
    public string OnProperty1ChangedCalled;

    [OnChangedMethod(nameof(OnProperty1Changed))]
    public string Property1 { get; set; }

    public void OnProperty1Changed(int before, int after)
    {
        OnProperty1ChangedCalled = before + "-" + after;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedBeforeAfterTypedGeneric<T> :
    INotifyPropertyChanged
{
    public string OnProperty1ChangedCalled;

    [OnChangedMethod(nameof(OnProperty1Changed))]
    public T Property1 { get; set; }

    public void OnProperty1Changed(T before, T after)
    {
        OnProperty1ChangedCalled = before + "-" + after;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class ClassWithOnChangedBeforeAfterTypedGenericString : ClassWithOnChangedBeforeAfterTypedGeneric<string>
{
}

public class ClassWithOnChangedBeforeAfterTypedGenericInteger : ClassWithOnChangedBeforeAfterTypedGeneric<int>
{
}
