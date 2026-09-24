using System.Reflection;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithInterceptorTests
{
    [Test]
    public async Task Simple()
    {
        var task = new ModuleWeaver();
        var testResult = task.ExecuteTestRun(
            "AssemblyWithInterceptor.dll",
            ignoreCodes: ["0x80131869"]);

        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public)!;
        var value = (bool)propertyInfo.GetValue(null, null);
        await Assert.That(value).IsTrue();
    }

    [Test]
    public async Task BeforeAfter()
    {
        var weaver = new ModuleWeaver();
        var testResult = weaver.ExecuteTestRun(
            "AssemblyWithBeforeAfterInterceptor.dll",
            ignoreCodes: ["0x80131869"]);
        var assembly = testResult.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public)!;
        var value = (bool)propertyInfo.GetValue(null, null);
        await Assert.That(value).IsTrue();
    }
}