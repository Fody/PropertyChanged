using System.ComponentModel;
using PropertyChanged;

public class ClassWithInvalidOnChanged : INotifyPropertyChanged
{
    public string PropertyWithInvalidOnChangedMethod { get; set; }
    public string PropertyWithInvalidOnChangedMethodSuppressed { get; set; }
    public string PropertyWithValidOnChangedMethod { get; set; }
    
    [DoNotNotify]
    public string IgnoredProperty { get; set; }

    public void OnPropertyWithInvalidOnChangedMethodChanged(int foo)
    {
    }

    [SuppressPropertyChangedWarnings]
    public void OnPropertyWithInvalidOnChangedMethodSuppressedChanged(int foo)
    {
    }

    public void OnPropertyWithValidOnChangedMethodChanged()
    {
    }
    
    public void OnNonExistingPropertyChanged()
    {
    }

    [SuppressPropertyChangedWarnings]
    public void OnNonExistingPropertySuppressedChanged()
    {
    }
    
    public void OnIgnoredPropertyChanged()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
