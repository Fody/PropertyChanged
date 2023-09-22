using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ClassWithNotifyInChildByAttribute : ParentClass
{
    public string Property { get; set; }
}