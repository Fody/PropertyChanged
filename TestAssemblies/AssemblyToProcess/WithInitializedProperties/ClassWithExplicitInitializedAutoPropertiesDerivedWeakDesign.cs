using PropertyChanged;

public class ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign : ClassWithExplicitInitializedAutoProperties
{
    public ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign()
    {
        Property1 = "test";
        Property2 = "test2";
        Property3 = "test3";
    }

    public string Property3 { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesignAndNotifyAutoPropertyOptOut : ClassWithExplicitInitializedAutoPropertiesAndNotifyAutoPropertyOptOut
{
    public ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesignAndNotifyAutoPropertyOptOut()
    {
        Property1 = "test";
        Property2 = "test2";
        Property3 = "test3";
    }

    public string Property3 { get; set; }
}

