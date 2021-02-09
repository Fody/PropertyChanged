using Fody;
using Xunit;

public class AssemblyWithBlockingClassTests
{
    [Fact]
    public void TestClassIsNotBlocked()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun(
            "AssemblyWithBlockingClass.dll",
            ignoreCodes: new[] {"0x80131869"});
        var instance = testResult.GetInstance("B");
        EventTester.TestProperty(instance, false);
    }
}