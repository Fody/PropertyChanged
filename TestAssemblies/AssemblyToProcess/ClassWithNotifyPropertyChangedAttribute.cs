using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ClassWithNotifyPropertyChangedAttribute
{
    public string Property1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class ClassWithNotifyPropertyChangedAttributeGeneric<T>
{
    public string Property1 { get; set; }
}
