using System.ComponentModel;
using PropertyChanged;

[ImplementPropertyChanged]
public class ClassWithNotifyPropertyChangedAttribute
{
    public string Property1 { get; set; }
}

[ImplementPropertyChanged]
public class ClassWithNotifyPropertyChangedAttributeGeneric<T>
{
    public string Property1 { get; set; }
}

public class BaseClass {}

public class BaseClassWithNotifyPropertyChanged : BaseClass, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
}

[ImplementPropertyChanged]
public class ClassWithBaseAndNotifyPropertyChangedAttribute : BaseClassWithNotifyPropertyChanged
{
    public string Property1 { get; set; }
}