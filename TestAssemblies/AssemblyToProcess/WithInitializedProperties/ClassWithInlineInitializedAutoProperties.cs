public class ClassWithInlineInitializedAutoProperties : ObservableTestObject
{
    public string Property1 { get; set; } = "Test";

    public string Property2 { get; set; } = "Test2";

    public bool IsChanged { get; set; }
}