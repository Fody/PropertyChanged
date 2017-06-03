using PropertyChanged;

[AddINotifyPropertyChangedInterfaceAttribute]
public class ClassWithNotifyInChildByAttribute : ParentClass
{
    public string Property { get; set; }
}