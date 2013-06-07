using PropertyChanged;

[ImplementPropertyChanged]
public class ClassWithNotifyInChildByAttribute : ParentClass
{
    public string Property { get; set; }
}