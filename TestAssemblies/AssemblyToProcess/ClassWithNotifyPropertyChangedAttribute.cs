using PropertyChanged;

[NotifyPropertyChanged]
public class ClassWithNotifyPropertyChangedAttribute
{
    public string Property1 { get; set; }
}
[NotifyPropertyChanged]
public class ClassWithNotifyPropertyChangedAttributeChild : ClassWithNotifyPropertyChangedAttributeChildParent
{
    public string Property1 { get; set; }
}
[NotifyPropertyChanged]
public class ClassWithNotifyPropertyChangedAttributeChildParent
{
    public string Property2 { get; set; }
}