using GalaSoft.MvvmLight;

using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class WithInlineInitializedAutoProperties
{
    public string Property1 { get; set; } = "Test";

    public string Property2 { get; set; } = "Test2";

    public bool IsChanged { get; set; }
}

[AddINotifyPropertyChangedInterface]
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

[NotifyAutoPropertiesInConstructor(true)]
public class WithExplicitConstructorInitializedAutoPropertiesAndClassLevelOptOut : ObservableObject
{
    public WithExplicitConstructorInitializedAutoPropertiesAndClassLevelOptOut()
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

public class WithExplicitConstructorInitializedAutoPropertiesAndMethodLevelOptOut : ObservableObject
{
    [NotifyAutoPropertiesInConstructor(true)]
    public WithExplicitConstructorInitializedAutoPropertiesAndMethodLevelOptOut()
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

[AddINotifyPropertyChangedInterface]
[NotifyAutoPropertiesInConstructor(true)]
public class WithExplicitConstructorInitializedAutoPropertiesAndClassLevelOptOutAndMethodLevelOptIn
{
    [NotifyAutoPropertiesInConstructor(false)]
    public WithExplicitConstructorInitializedAutoPropertiesAndClassLevelOptOutAndMethodLevelOptIn()
    {
        Property1 = "Test";
        Property2 = "Test2";
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public bool IsChanged { get; set; }
}

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
