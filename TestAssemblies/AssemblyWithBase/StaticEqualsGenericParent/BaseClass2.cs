namespace AssemblyWithBase.StaticEqualsGenericParent;

public class BaseClass2<T, TSomething> : BaseClass<TSomething>
{
    public T SomeProperty { get; set; }
}