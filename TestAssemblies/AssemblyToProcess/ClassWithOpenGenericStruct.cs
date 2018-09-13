using System.Collections.Generic;

using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ClassWithOpenGenericStruct<T>
{
    public KeyValuePair<string, T> Property1 { get; set; }
}
