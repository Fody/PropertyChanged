using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithDisabledTriggerDependentPropertiesTests
{
    static TestResult testResult;

    static AssemblyWithDisabledTriggerDependentPropertiesTests()
    {
        var task = new ModuleWeaver
        {
            TriggerDependentProperties = false
        };
        testResult = task.ExecuteTestRun(
            "AssemblyWithDisabledTriggerDependentProperties.dll",
            ignoreCodes: ["0x80131869"]);
    }

    [Test]
    public async Task TriggerDependentPropertiesDisabled()
    {
        var instance = testResult.GetInstance(nameof(DependentPropertiesClassToTest));
        instance.Property1 = "foo";

        await Assert.That((int)instance.OnProperty1ChangedCallCount).IsEqualTo(1);
        await Assert.That((int)instance.OnProperty2ChangedCallCount).IsEqualTo(0);
    }
}
