// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithBlockingClassTests
{
    [Test]
    public void TestClassIsNotBlocked()
    {
        var task = new ModuleWeaver();
        var testResult = task.ExecuteTestRun(
            "AssemblyWithBlockingClass.dll",
            ignoreCodes: ["0x80131869"]);
        var instance = testResult.GetInstance("B");
        EventTester.TestProperty(instance, false);
    }
}