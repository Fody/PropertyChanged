public class ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign : ClassWithExplicitInitializedAutoProperties
{
    public ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign()
        : base((string) "test", (string) "test2")
    {
        Property3 = "test3";
    }

    public string Property3 { get; set; }
}