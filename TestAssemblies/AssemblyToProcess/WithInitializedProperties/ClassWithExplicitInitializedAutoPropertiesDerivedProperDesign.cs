using PropertyChanged;

public class ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign : ClassWithExplicitInitializedAutoProperties
{
    public ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign()
        : base("test", "test2")
    {
        Property3 = "test3";
    }

    public string Property3 { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class ClassWithExplicitInitializedAutoPropertiesDerivedProperDesignAndNotifyAutoPropertyOptOut : ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOut
{
    public ClassWithExplicitInitializedAutoPropertiesDerivedProperDesignAndNotifyAutoPropertyOptOut()
        : base("test", "test2")
    {
        Property3 = "test3";
    }

    public string Property3 { get; set; }
}

