using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Fody;

using Xunit;
using Xunit.Abstractions;

public class AssemblyWithInheritanceTests
{
    static AssemblyWithInheritanceTests()
    {
        var weavingTask = new ModuleWeaver();

        var testResults = new[]
        {
            weavingTask.ExecuteTestRun("AssemblyWithInheritance.dll"),
            weavingTask.ExecuteTestRun("AssemblyWithExternalInheritance.dll")
        };

        // Must use Assembly.LoadFrom, else the non-woven assemblies will be loaded as dependencies!
        assemblies = testResults
            .Select(result => result.AssemblyPath)
            .Select(path => Assembly.LoadFrom(path))
            .ToArray();
    }

    public AssemblyWithInheritanceTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
    }

    [Theory]
    [InlineData(0, "DerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed" })]
    [InlineData(0, "DerivedClass", "Property2", new[] { "Property5", "derived:OnProperty2Changed", "Property2" })]
    [InlineData(0, "DerivedClass", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedNoOverrides", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedNoOverrides", "Property2", new[] { "Property4", "base:OnProperty2Changed", "Property2" })]
    [InlineData(0, "DerivedNoOverrides", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedDerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed", "Property6", "derived++:OnProperty1Changed" })]
    [InlineData(0, "DerivedDerivedClass", "Property2", new[] { "Property6", "derived++:OnProperty2Changed", "Property2" })]
    [InlineData(0, "DerivedDerivedClass", "Property3", new[] { "Property6", "derived++:OnProperty3Changed", "Property3" })]
    [InlineData(1, "DerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed" })]
    [InlineData(1, "DerivedClass", "Property2", new[] { "Property5", "derived:OnProperty2Changed", "Property2" })]
    [InlineData(1, "DerivedClass", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(1, "DerivedNoOverrides", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(1, "DerivedNoOverrides", "Property2", new[] { "Property4", "base:OnProperty2Changed", "Property2" })]
    [InlineData(1, "DerivedNoOverrides", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "PocoBase", "Property1", new string[0])]
    [InlineData(0, "DerivedFromPoco", "Property1", new[] { "Property4", "derived:OnProperty1Changed", "Property1" })]
    public void DerivedClassRaisesAllExpectedEvents(int assemblyIndex, string className, string propertyName, string[] expected)
    {
        var assembly = assemblies[assemblyIndex];
        var instanceType = assembly.GetType(className);
        var instance = (dynamic)Activator.CreateInstance(instanceType);

        var actual = (IList<string>)instance.Notifications;

        instanceType.GetProperty(propertyName)?.SetValue(instance, 42);

        outputHelper.WriteLine(assembly.CodeBase);
        outputHelper.WriteLine(string.Join(", ", actual.Select(item => $"\"{item}\"")));

        Assert.Equal(expected, actual);
    }

    readonly ITestOutputHelper outputHelper;
    static Assembly[] assemblies;
}
