using System.CodeDom.Compiler;
using System.ComponentModel;
using PropertyChanged;

public class ClassWithGeneratedPropertyChangedBase;

[AddINotifyPropertyChangedInterface]
public class ClassWithGeneratedPropertyChanged : ClassWithGeneratedPropertyChangedBase, INotifyPropertyChanged
{
    public string Property1 { get; set; }

    [GeneratedCode("PropertyChanged.Fody", "TEST")]
    public event PropertyChangedEventHandler PropertyChanged;
}