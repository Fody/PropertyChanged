public class AssemblyWithBlockingClassTests
{
    [Fact]
    public void TestClassIsNotBlocked()
    {
        var task = new ModuleWeaver();
        var testResult = task.ExecuteTestRun(
            "AssemblyWithBlockingClass.dll",
            ignoreCodes: new[] {"0x80131869"});
        var instance = testResult.GetInstance("B");
        EventTester.TestProperty(instance, false);
    }
}