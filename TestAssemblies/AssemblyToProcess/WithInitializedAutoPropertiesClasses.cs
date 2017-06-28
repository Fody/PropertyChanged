using GalaSoft.MvvmLight;

using PropertyChanged;

[AddINotifyPropertyChangedInterface]
[NotifyAutoPropertiesInConstructor(false)]
public class WithInlineInitializedAutoProperties
{
    public string Property1 { get; set; } = "Test";

    public string Property2 { get; set; } = "Test2";

    public bool IsChanged { get; set; }
}

[AddINotifyPropertyChangedInterface]
[NotifyAutoPropertiesInConstructor(false)]
public class WithExplicitConstructorInitializedAutoProperties
{
    public WithExplicitConstructorInitializedAutoProperties()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    protected WithExplicitConstructorInitializedAutoProperties(string property1, string property2)
    {
        Property1 = property1;
        Property2 = property2;
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

public class WithExplicitConstructorInitializedAutoPropertiesNoOptOut : ObservableObject
{
    public WithExplicitConstructorInitializedAutoPropertiesNoOptOut()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }

    [DoNotNotify]
    public int PropertyChangedCalls { get; private set; }

    public override void RaisePropertyChanged(string propertyName)
    {
        base.RaisePropertyChanged(propertyName);

        PropertyChangedCalls += 1;
    }
}

public class WithExplicitConstructorInitializedAutoPropertiesAndPartialPropertyLevelOptOut : ObservableObject
{
    public WithExplicitConstructorInitializedAutoPropertiesAndPartialPropertyLevelOptOut()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    [NotifyAutoPropertiesInConstructor(true)]
    public string Property1 { get; set; }

    [NotifyAutoPropertiesInConstructor(false)]
    public string Property2 { get; set; }

    public bool IsChanged { get; set; }

    [DoNotNotify]
    public int PropertyChangedCalls { get; private set; } = 1; // we get one call less if only one property does opt-out. start at 1 so we can use the same test as with both properties.

    public override void RaisePropertyChanged(string propertyName)
    {
        base.RaisePropertyChanged(propertyName);

        PropertyChangedCalls += 1;
    }
}

[AddINotifyPropertyChangedInterface]
public class WithExplicitConstructorInitializedAutoPropertiesAndMethodLevelOptIn
{
    [NotifyAutoPropertiesInConstructor(false)]
    public WithExplicitConstructorInitializedAutoPropertiesAndMethodLevelOptIn()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class WithExplicitConstructorInitializedAutoPropertiesAndPropertyLevelOptIn
{
    public WithExplicitConstructorInitializedAutoPropertiesAndPropertyLevelOptIn()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    [NotifyAutoPropertiesInConstructor(false)]
    public string Property1 { get; set; }

    [NotifyAutoPropertiesInConstructor(false)]
    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class WithExplicitConstructorInitializedAutoPropertiesDerivedWeakDesign : WithExplicitConstructorInitializedAutoProperties
{
    public WithExplicitConstructorInitializedAutoPropertiesDerivedWeakDesign()
    {
        Property1 = "Derived";
        Property2 = "Derived2";
        Property3 = "Test3";
    }

    public string Property3 { get; set; }
}

[NotifyAutoPropertiesInConstructor(false)]
public class WithExplicitConstructorInitializedAutoPropertiesDerivedProperDesign : WithExplicitConstructorInitializedAutoProperties
{
    public WithExplicitConstructorInitializedAutoPropertiesDerivedProperDesign()
        : base("Derived", "Derived2")
    {
        Property3 = "Test3";
    }

    public string Property3 { get; set; }
}

[AddINotifyPropertyChangedInterface]
[NotifyAutoPropertiesInConstructor(false)]
public class WithMethodInitializedAutoProperties
{
    public WithMethodInitializedAutoProperties()
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

[NotifyAutoPropertiesInConstructor(false)]
public class WithObservableBaseClass : ObservableObject
{
    public string Property1 { get; set; } = "Test";

    public string Property2 { get; set; } = "Test2";

    [DoNotNotify]
    public int VirtualMethodCalls { get; private set; }

    public override void RaisePropertyChanged(string propertyName)
    {
        base.RaisePropertyChanged(propertyName);

        VirtualMethodCalls += 1;
    }
}

[NotifyAutoPropertiesInConstructor(false)]
public class WithBackingFieldsAndPropertySetterInConstructor : ObservableObject
{
    private string _property1;
    private string _property2;

    public WithBackingFieldsAndPropertySetterInConstructor()
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

    [DoNotNotify]

    public int VirtualMethodCalls { get; private set; }

    public override void RaisePropertyChanged(string propertyName)
    {
        base.RaisePropertyChanged(propertyName);

        VirtualMethodCalls += 1;
    }
}


