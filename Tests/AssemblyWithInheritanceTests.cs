using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

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

#if NETFRAMEWORK
        assemblies = testResults
            // Must use Assembly.LoadFrom, else the non-woven assemblies will be loaded as dependencies!
            .Select(result => Assembly.LoadFrom(result.AssemblyPath))
            .ToArray();
#else
        var folder = Path.GetDirectoryName(testResults.First().AssemblyPath);
        var loadContext = new PluginLoadContext(folder);
        assemblies = testResults
            .Select(result => loadContext.LoadFromAssemblyPath(result.AssemblyPath))
            .ToArray();
#endif
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
    [InlineData(0, "DerivedNewProperties", "Property1", new[] { "Property5", "derived:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedNewProperties", "Property2", new[] { "Property4", "base:OnProperty2Changed", "Property2" })]
    [InlineData(0, "DerivedNewProperties", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
    [InlineData(0, "DerivedCallingChild", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1" })]
    [InlineData(0, "DerivedCallingChild", "Property2", new[] { "Property2" })]
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

#if NETCOREAPP
class PluginLoadContext : AssemblyLoadContext
{
    AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
#endif