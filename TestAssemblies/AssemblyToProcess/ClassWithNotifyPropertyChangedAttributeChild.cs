using PropertyChanged;

[ImplementPropertyChanged]
public class ClassWithNotifyPropertyChangedAttributeChild : ClassWithNotifyPropertyChangedAttributeChildParent
{
    public string Property1 { get; set; }
}