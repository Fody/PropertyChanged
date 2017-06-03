using PropertyChanged;

[AddINotifyPropertyChangedInterfaceAttribute]
public class ClassWithNotifyPropertyChangedAttribute
{
    public string Property1 { get; set; }
}

[AddINotifyPropertyChangedInterfaceAttribute]
public class ClassWithNotifyPropertyChangedAttributeGeneric<T>
{
    public string Property1 { get; set; }
}
