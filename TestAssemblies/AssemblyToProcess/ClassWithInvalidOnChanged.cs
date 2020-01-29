using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PropertyChanged;

public class ClassWithInvalidOnChanged :
    INotifyPropertyChanged
{
    public string PropertyWithInvalidOnChangedMethod { get; set; }
    public string PropertyWithInvalidOnChangedMethodSuppressed { get; set; }
    public string PropertyWithValidOnChangedMethod { get; set; }

    public const string IndexerName = "Item";
    [IndexerName(IndexerName)]
    [SuppressPropertyChangedWarnings]
    [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
    public string this[string index] { get => null; set { } }

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

[SuppressPropertyChangedWarnings]
public class ClassWithSuppressedInvalidOnChanged
  : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnNonExistingPropertyChanged()
    {
    }
}
