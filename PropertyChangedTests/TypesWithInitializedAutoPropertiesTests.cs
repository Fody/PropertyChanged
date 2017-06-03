using System.ComponentModel;

using NUnit.Framework;

public abstract partial class BaseTaskTests
{
    [Test]
    [TestCase("WithInlineInitializedAutoProperties")]
    [TestCase("WithExplicitConstructorInitializedAutoProperties")]
    public void WithInitializedAutoPropertiesTest(string className)
    {
        var instance = assembly.GetInstance(className);

        var propertyEventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            propertyEventCount++;
        };

        Assert.IsFalse(instance.IsChanged);
        Assert.AreEqual("Test", instance.Property1);
        Assert.AreEqual("Test2", instance.Property2);

        // setting any property inplicitly sets "IsChanged", so we get 2 events!

        instance.Property1 = "a";
        Assert.AreEqual(2, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);

        instance.IsChanged = false;
        Assert.AreEqual(3, propertyEventCount);

        instance.Property2 = "b";
        Assert.AreEqual(5, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);
    }

    [Test]
    public void WithInitializedAutoPropertiesDerivedWeakDesignTest()
    {
        var instance = assembly.GetInstance("WithExplicitConstructorInitializedAutoPropertiesDerivedWeakDesign");

        var propertyEventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            propertyEventCount++;
        };

        // weak class design: derived class can't access backing field of base class
        Assert.IsTrue(instance.IsChanged);
        Assert.AreEqual("Derived", instance.Property1);
        Assert.AreEqual("Derived2", instance.Property2);

        // setting any property inplicitly sets "IsChanged", so we sometimes get 2 events!

        instance.Property1 = "a";
        Assert.AreEqual(1, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);

        instance.IsChanged = false;
        Assert.AreEqual(2, propertyEventCount);

        instance.Property2 = "b";
        Assert.AreEqual(4, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);
    }

    [Test]
    public void WithInitializedAutoPropertiesDerivedProperDesignTest()
    {
        var instance = assembly.GetInstance("WithExplicitConstructorInitializedAutoPropertiesDerivedProperDesign");

        var propertyEventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            propertyEventCount++;
        };

        // works with derived classes if class design is properly done...
        Assert.IsFalse(instance.IsChanged);
        Assert.AreEqual("Derived", instance.Property1);
        Assert.AreEqual("Derived2", instance.Property2);

        // setting any property inplicitly sets "IsChanged", so we sometimes get 2 events!

        instance.Property1 = "a";
        Assert.AreEqual(2, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);

        instance.IsChanged = false;
        Assert.AreEqual(3, propertyEventCount);

        instance.Property2 = "b";
        Assert.AreEqual(5, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);
    }

    [Test]
    public void WithInitializedAutoPropertiesInMethodTest()
    {
        var instance = assembly.GetInstance("WithMethodInitializedAutoProperties");

        var propertyEventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            propertyEventCount++;
        };

        // setting a property from within an extra method is not handled.
        Assert.IsTrue(instance.IsChanged);

        Assert.AreEqual("Test", instance.Property1);
        Assert.AreEqual("Test2", instance.Property2);

        // setting any property inplicitly sets "IsChanged", so we get 2 events!

        instance.Property1 = "a";
        Assert.AreEqual(1, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);

        instance.IsChanged = false;
        Assert.AreEqual(2, propertyEventCount);

        instance.Property2 = "b";
        Assert.AreEqual(4, propertyEventCount);
        Assert.IsTrue(instance.IsChanged);
    }

    [Test]
    public void WithInitializedAutoPropertiesAndINPCImplementationFromBaseClassTest()
    {
        var instance = assembly.GetInstance("WithObservableBaseClass");

        Assert.AreEqual("Test", instance.Property1);
        Assert.AreEqual("Test2", instance.Property2);
        Assert.AreEqual(0, instance.VirtualMethodCalls);

        instance.Property1 = "a";
        Assert.IsTrue(instance.BaseNotifyCalled); // has changed indirectly => 2 VM calls.
        Assert.AreEqual(2, instance.VirtualMethodCalls);

        instance.Property2 = "b";
        Assert.AreEqual(3, instance.VirtualMethodCalls);
    }
    [Test]
    public void WithInitializedBackingFieldPropertiesAndINPCImplementationFromBaseClassTest()
    {
        var instance = assembly.GetInstance("WithBackingFieldsAndPropertySetterInConstructor");

        Assert.AreEqual("Test", instance.Property1);
        Assert.AreEqual("Test2", instance.Property2);
        Assert.IsTrue(instance.BaseNotifyCalled); // has changed indirectly => 3 VM calls.
        Assert.AreEqual(3, instance.VirtualMethodCalls);

        instance.Property1 = "a";
        Assert.AreEqual(4, instance.VirtualMethodCalls);

        instance.Property2 = "b";
        Assert.AreEqual(5, instance.VirtualMethodCalls);
    }
}
