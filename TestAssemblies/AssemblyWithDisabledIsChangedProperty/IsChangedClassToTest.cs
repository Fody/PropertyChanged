using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "NotAccessedField.Global")]
public class IsChangedClassToTest :
    INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}