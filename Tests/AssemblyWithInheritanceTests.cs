using System;
using System.Collections.Generic;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

using Xunit.Abstractions;

public class AssemblyWithInheritanceTests(ITestOutputHelper outputHelper)
{
    static AssemblyWithInheritanceTests()
    {
        var task = new ModuleWeaver();

        var testResults = new[]
        {
            task.ExecuteTestRun("AssemblyWithInheritance.dll", ignoreCodes: peVerifyIgnoreCodes),
            task.ExecuteTestRun("AssemblyWithExternalInheritance.dll", ignoreCodes: peVerifyIgnoreCodes)
        };

#if NETFRAMEWORK
        assemblies = testResults
            // Must use Assembly.LoadFrom, else the non-woven assemblies will be loaded as dependencies!
            .Select(result => Assembly.LoadFrom(result.AssemblyPath))
            .ToArray();
#else
        var loadContext = new AssemblyLoadContext(null);
        assemblies = testResults
            .Select(result => loadContext.LoadFromAssemblyPath(result.AssemblyPath))
            .ToArray();
#endif
    }

    [Theory]
    [InlineData(0, "DerivedClass", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1", "derived:OnProperty5Changed", "Property5", "derived:OnProperty1Changed" })]
    [InlineData(0, "DerivedClass", "Property2", new[] { "derived:OnProperty5Changed", "Property5", "derived:On_Property2_Changed", "Property2" })]
    [InlineData(0, "DerivedClass", "Property3", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedNoOverrides", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedNoOverrides", "Property2", new[] { "base:OnProperty4Changed", "Property4", "base:On_Property2_Changed", "Property2" })]
    [InlineData(0, "DerivedNoOverrides", "Property3", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedDerivedClass", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1", "derived:OnProperty5Changed", "Property5", "derived:OnProperty1Changed", "derived++:OnProperty6Changed", "Property6", "derived++:OnProperty1Changed" })]
    [InlineData(0, "DerivedDerivedClass", "Property2", new[] { "derived++:OnProperty6Changed", "Property6", "derived++:OnProperty2Changed", "Property2" })]
    [InlineData(0, "DerivedDerivedClass", "Property3", new[] { "derived++:OnProperty6Changed", "Property6", "derived++:OnProperty3Changed", "Property3" })]
    [InlineData(1, "DerivedClass", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1", "derived:OnProperty5Changed", "Property5", "derived:OnProperty1Changed" })]
    [InlineData(1, "DerivedClass", "Property2", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty2Changed", "Property2" })]
    [InlineData(1, "DerivedClass", "Property3", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(1, "DerivedNoOverrides", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(1, "DerivedNoOverrides", "Property2", new[] { "base:OnProperty4Changed", "Property4", "base:On_Property2_Changed", "Property2" })]
    [InlineData(1, "DerivedNoOverrides", "Property3", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "PocoBase", "Property1", new string[0])]
    [InlineData(0, "DerivedFromPoco", "Property1", new[] { "derived:OnProperty4Changed", "Property4", "derived:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedNewProperties", "Property1", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedNewProperties", "Property2", new[] { "base:OnProperty4Changed", "Property4", "base:On_Property2_Changed", "Property2" })]
    [InlineData(0, "DerivedNewProperties", "Property3", new[] { "derived:OnProperty5Changed", "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedCallingChild", "Property1", new[] { "base:OnProperty4Changed", "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedCallingChild", "Property2", new[] { "Property2" })]
    [InlineData(0, "DerivedClass", "Property9", new[] { "base:On_Property9_Changed", "Property9" })]
    public void DerivedClassRaisesAllExpectedEvents(int assemblyIndex, string className, string propertyName, string[] expected)
    {
        var assembly = assemblies[assemblyIndex];
        var instanceType = assembly.GetType(className);
        var instance = (dynamic)Activator.CreateInstance(instanceType);


        instanceType.GetProperty(propertyName)?.SetValue(instance, 42);

        var actual = (IList<string>)instance.Notifications;

        var act = string.Join(", ", actual.Select(item => $"\"{item}\""));
        var exp = string.Join(", ", expected.Select(item => $"\"{item}\""));

        outputHelper.WriteLine(assembly.Location);
        outputHelper.WriteLine("");
        outputHelper.WriteLine($"EXPECTED {exp}");
        outputHelper.WriteLine($"ACTUAL       {act}");
        outputHelper.WriteLine("");

        Assert.Equal(expected, actual);
    }

    static Assembly[] assemblies;
    static readonly string[] peVerifyIgnoreCodes =
    {
#if NETCOREAPP
        "0x80131869"
#endif
    };
}
