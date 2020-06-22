using Fody;
using Xunit;

public class AssemblyWithBlockingClassTests
{
    [Fact]
    public void TestClassIsNotBlocked()
    {
        var weavingTask = new ModuleWeaver();
        var testResult = weavingTask.ExecuteTestRun("AssemblyWithBlockingClass.dll");
        var instance = testResult.GetInstance("B");
        EventTester.TestProperty(instance, false);
    }
}