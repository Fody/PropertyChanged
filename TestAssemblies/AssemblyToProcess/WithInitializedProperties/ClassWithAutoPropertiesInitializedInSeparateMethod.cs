public class ClassWithAutoPropertiesInitializedInSeparateMethod : ObservableTestObject
{
    public ClassWithAutoPropertiesInitializedInSeparateMethod()
    {
        Init();
    }

    private void Init()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}