﻿using System;
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
    public void ClassWithTaskReturningPropertyChangedNotifier()
    {
        var instance = testResult.GetInstance("ClassWithTaskReturningPropertyChangedNotifier");
        EventTester.TestProperty(instance, false, true);
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
        var instance = testResult.GetInstance("ClassReactiveUI2");
        
        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property2 = "b";
        
        Assert.Equal(2, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property2", argsList[1].PropertyName);
    }

    [Fact]
    public void ClassWithOpenGenerics()
    {
        var instance = testResult.GetGenericInstance("ClassWithOpenGenerics`1", typeof(int));

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        var value1 = new KeyValuePair<string, int>("a", 1);

        instance.Property1 = value1;

        Assert.Single(argsList);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal(value1, instance.Property1);
        Assert.Equal(new KeyValuePair<string, int>("a", 1), instance.Property1);

        instance.Property1 = new KeyValuePair<string, int>("a", 1);

        Assert.Single(argsList);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal(value1, instance.Property1);
        Assert.Equal(new KeyValuePair<string, int>("a", 1), instance.Property1);

        instance.Property1 = new KeyValuePair<string, int>("a", 2);

        Assert.Equal(2, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.NotEqual(value1, instance.Property1);
        Assert.NotEqual(new KeyValuePair<string, int>("a", 1), instance.Property1);

        var value2 = new Tuple<string, int>("b", 2);

        instance.Property2 = value2;

        Assert.Equal(3, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal(value2, instance.Property2);
        Assert.Equal(new Tuple<string, int>("b", 2), instance.Property2);

        instance.Property2 = new Tuple<string, int>("b", 2);

        Assert.Equal(3, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal(value2, instance.Property2);
        Assert.Equal(new Tuple<string, int>("b", 2), instance.Property2);

        instance.Property2 = new Tuple<string, int>("b", 1);

        Assert.Equal(4, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal("Property2", argsList[3].PropertyName);
        Assert.NotEqual(value2, instance.Property2);
        Assert.NotEqual(new Tuple<string, int>("b", 2), instance.Property2);
    }
}