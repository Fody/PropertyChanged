using System.Reflection;
using Fody;
using Xunit;

public class AssemblyWithInterceptorTests
{
    [Fact]
    public void Simple()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun(
            "AssemblyWithInterceptor.dll",
            ignoreCodes: new[] {"0x80131869"});

        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public)!;
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.True(value);
    }

    [Fact]
    public void BeforeAfter()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun(
            "AssemblyWithBeforeAfterInterceptor.dll",
            ignoreCodes: new[] {"0x80131869"});
        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public)!;
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.True(value);
    }
}