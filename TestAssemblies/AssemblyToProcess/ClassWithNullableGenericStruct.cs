using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ParentOfNullableGenericStruct<T>
{
    public GenericStruct<T>? ChildProperty { get; set; }
}

public struct GenericStruct<T>
{
    public T Value { get; }
}
