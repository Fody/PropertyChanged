using AssemblyWithBase.DirectGeneric;

namespace AssemblyWithBaseInDifferentModule.DirectGeneric
{
    public class ChildClass : BaseClass<object>
    {
        public string Property1 { get; set; }

    }
}