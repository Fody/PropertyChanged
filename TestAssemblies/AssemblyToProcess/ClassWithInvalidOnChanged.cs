using System.ComponentModel;
using PropertyChanged;

public class ClassWithInvalidOnChanged : INotifyPropertyChanged
{
    public string PropertyWithInvalidOnChangedMethod { get; set; }
    public string PropertyWithInvalidOnChangedMethodSuppressed { get; set; }
    public string PropertyWithValidOnChangedMethod { get; set; }
    
    public void OnPropertyWithInvalidOnChangedMethodChanged (int foo)
    {
    }

    [SuppressPropertyChangedWarnings]
    public void OnPropertyWithInvalidOnChangedMethodSuppressedChanged(int foo)
    {
    }
    
    public void OnPropertyWithValidOnChangedMethodChanged ()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
