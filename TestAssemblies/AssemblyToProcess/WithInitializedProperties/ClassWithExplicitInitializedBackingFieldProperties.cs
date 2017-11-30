public class ClassWithExplicitInitializedBackingFieldProperties : ObservableTestObject
{
    string property1;
    string property2;

    public ClassWithExplicitInitializedBackingFieldProperties()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1    {
        get => property1;        set => property1 = value;    }

    public string Property2    {
        get => property2;        set => property2 = value;    }

    public bool IsChanged { get; set; }
}