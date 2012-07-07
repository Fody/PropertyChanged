
namespace GenericChildWithPropertyOnChanged
{
    public class ClassWithGenericPropertyChild : ClassWithGenericPropertyParent<string>
    {
        public bool OnProperty1ChangedCalled;

        public string Property1 { get; set; }
        public void OnProperty1Changed()
        {
            OnProperty1ChangedCalled = true;
        }

        public bool OnProperty2ChangedCalled;

        public string Property2 { get; set; }
        public void OnProperty2Changed()
        {
            OnProperty2ChangedCalled = true;
        }
    }
}