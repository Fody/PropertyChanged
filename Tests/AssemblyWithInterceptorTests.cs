using System.Reflection;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class AssemblyWithInterceptorTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun("AssemblyWithInterceptor.dll");

        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.True(value);
    }

    [Fact]
    public void BeforeAfter()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun("AssemblyWithBeforeAfterInterceptor.dll");
        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.True(value);
    }

    public AssemblyWithInterceptorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}