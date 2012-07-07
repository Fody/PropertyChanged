using AssemblyWithBase.MultiTypes;

namespace AssemblyWithBaseInDifferentModule.MultiTypes
{
    public class ChildClass : BaseClass2<int>
    {
        public string Property1 { get; set; }

    }
}