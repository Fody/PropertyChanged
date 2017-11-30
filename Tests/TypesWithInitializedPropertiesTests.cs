using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

public class TypesWithInitializedPropertiesTests
{
    const string assemblyName = "AssemblyToProcess";
    readonly Assembly assembly = new WeaverHelper(assemblyName).Assembly;

    [Test]
    [TestCase("ClassWithInlineInitializedAutoProperties", 
        "Test", "Test2", false, new string[0])]
    [TestCase("ClassWithExplicitInitializedAutoProperties", 
        "Test", "Test2", true, new[]{ "IsChanged", "Property1", "Property2" })]
    [TestCase("ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign", 
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property1", "Property2", "Property3" })]
    [TestCase("ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign", 
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property3" })]
    [TestCase("ClassWithAutoPropertiesInitializedInSeparateMethod", 
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [TestCase("ClassWithExplicitInitializedBackingFieldProperties", 
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    public void TypesWithInitializedPropertiesTest(string className, string property1Value, string property2Value, bool isChangedStateAfterConstructor, string[] propertyChangedCallsInConstructor)
    {
        var instance = assembly.GetInstance(className);

        var eventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            eventCount++;
        };

        Assert.AreEqual(property1Value, instance.Property1);
        Assert.AreEqual(property2Value, instance.Property2);

        var actualPropertyChangedCalls = (IList<string>)instance.PropertyChangedCalls;
        Debug.WriteLine("PropertyChanged calls: " + string.Join(", ", actualPropertyChangedCalls));

        Assert.IsTrue(propertyChangedCallsInConstructor.SequenceEqual(actualPropertyChangedCalls));
        Assert.AreEqual(isChangedStateAfterConstructor, instance.IsChanged);

        var initial = isChangedStateAfterConstructor ? 1 : 2;

        instance.Property1 = "a";
        Assert.AreEqual(initial, eventCount);
        Assert.IsTrue(instance.IsChanged);

        instance.IsChanged = false;
        Assert.AreEqual(initial + 1, eventCount);

        instance.Property2 = "b";
        Assert.AreEqual(initial + 3, eventCount);
        Assert.IsTrue(instance.IsChanged);
    }
}