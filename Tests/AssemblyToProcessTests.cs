using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Fody;
using Xunit;
#pragma warning disable 618

public class AssemblyToProcessTests
{
    static TestResult testResult;

    static AssemblyToProcessTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll"
#if NETCOREAPP2_0
            , runPeVerify:false
#endif
        );
    }

    [Theory]
    [InlineData("ClassWithInlineInitializedAutoProperties",
        "Test", "Test2", false, new string[0])]
    [InlineData("ClassWithExplicitInitializedAutoProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [InlineData("ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property1", "Property2", "Property3" })]
    [InlineData("ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property3" })]
    [InlineData("ClassWithAutoPropertiesInitializedInSeparateMethod",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [InlineData("ClassWithExplicitInitializedBackingFieldProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    public void TypesWithInitializedPropertiesTest(string className, string property1Value, string property2Value, bool isChangedStateAfterConstructor, string[] propertyChangedCallsInConstructor)
    {
        var instance = testResult.GetInstance(className);

        var eventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            eventCount++;
        };

        Assert.Equal(property1Value, instance.Property1);
        Assert.Equal(property2Value, instance.Property2);

        var actualPropertyChangedCalls = (IList<string>)instance.PropertyChangedCalls;
        Debug.WriteLine("PropertyChanged calls: " + string.Join(", ", actualPropertyChangedCalls));

        Assert.True(propertyChangedCallsInConstructor.SequenceEqual(actualPropertyChangedCalls));
        Assert.Equal(isChangedStateAfterConstructor, instance.IsChanged);

        var initial = isChangedStateAfterConstructor ? 1 : 2;

        instance.Property1 = "a";
        Assert.Equal(initial, eventCount);
        Assert.True(instance.IsChanged);

        instance.IsChanged = false;
        Assert.Equal(initial + 1, eventCount);

        instance.Property2 = "b";
        Assert.Equal(initial + 3, eventCount);
        Assert.True(instance.IsChanged);
    }

    [Fact]
    public void ClassWithIndirectImplementation()
    {
        var instance = testResult.GetInstance("ClassWithIndirectImplementation");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void UseSingleEventInstance()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangedAttribute");

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property1 = "b";

        Assert.Equal(2, argsList.Count);
        Assert.Same(argsList[0], argsList[1]);
    }

    [Fact]
    public void SupportedLibrariesClassReactiveUI()
    {
        var instance = testResult.GetInstance("ClassReactiveUI");
        
        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property1 = "b";
        
        Assert.Equal(2, argsList.Count);
        Assert.All(argsList, i => Assert.Equal("Property1", i.PropertyName));
    }
}