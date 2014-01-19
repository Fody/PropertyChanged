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
