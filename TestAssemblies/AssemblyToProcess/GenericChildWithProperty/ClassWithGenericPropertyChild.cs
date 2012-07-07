
namespace GenericChildWithProperty
{
    public class ClassWithGenericPropertyChild : ClassWithGenericPropertyParent<string>
    {
        public string Property1 { get; set; }
    }
}