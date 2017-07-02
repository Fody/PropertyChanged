using PropertyChanged;

public class ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOutInConstructor : ObservableTestObject
{
    [NotifyAutoPropertiesInConstructor(false)]
    public ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOutInConstructor()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOutAndOptInForProperty1 : ObservableTestObject
{
    public ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOutAndOptInForProperty1()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    [NotifyAutoPropertiesInConstructor(true)]
    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}