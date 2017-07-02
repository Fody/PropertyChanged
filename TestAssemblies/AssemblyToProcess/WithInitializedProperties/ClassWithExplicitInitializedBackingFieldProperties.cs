using PropertyChanged;

public class ClassWithExplicitInitializedBackingFieldProperties : ObservableTestObject
{
    private string _property1;
    private string _property2;

    public ClassWithExplicitInitializedBackingFieldProperties()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1
    {
        get
        {
            return _property1;
        }
        set
        {
            _property1 = value;
        }
    }

    public string Property2
    {
        get
        {
            return _property2;
        }
        set
        {
            _property2 = value;
        }
    }

    public bool IsChanged { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class ClassWithExplicitInitializedBackingFieldPropertiesAndNotifyAutoPropertyOptOut : ObservableTestObject
{
    private string _property1;
    private string _property2;

    public ClassWithExplicitInitializedBackingFieldPropertiesAndNotifyAutoPropertyOptOut()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1
    {
        get
        {
            return _property1;
        }
        set
        {
            _property1 = value;
        }
    }

    public string Property2
    {
        get
        {
            return _property2;
        }
        set
        {
            _property2 = value;
        }
    }

    public bool IsChanged { get; set; }
}

