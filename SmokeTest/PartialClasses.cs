namespace SmokeTest1;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PropertyChanged;
using SmokeTest;

public class SimpleClass
{
    public string P1 { get; set; }
}

public partial class Class1234 : INotifyPropertyChanged
{
    public string P1 { get; set; }
}

public partial class Class1234 // : INotifyPropertyChanged
{
    public string P2 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public partial class Class1234
{
    public string P3 { get; set; }
}

public partial class XYZ123 : INotifyPropertyChanged
{
    public string P3 { get; set; }
}

public partial class XYZ123<T> : INotifyPropertyChanged
{
    public T P3 { get; set; }
}

[AddINotifyPropertyChangedInterface]
[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
public partial class WithBase2 : Class2
{
    public string P1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class WithBase3 : Class3, INotifyPropertyChanged
{
    public string P1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public partial class WithBase4 : SimpleClass
{
    public string P2 { get; set; }
}