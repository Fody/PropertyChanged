using PropertyChanged;

public class ClassWithExplicitInitializedAutoProperties : ObservableTestObject
{
    public ClassWithExplicitInitializedAutoProperties()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    protected ClassWithExplicitInitializedAutoProperties(string property1, string property2)
    {
        Property1 = property1;
        Property2 = property2;
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOut : ObservableTestObject
{
    public ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOut()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    protected ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOut(string property1, string property2)
    {
        Property1 = property1;
        Property2 = property2;
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}