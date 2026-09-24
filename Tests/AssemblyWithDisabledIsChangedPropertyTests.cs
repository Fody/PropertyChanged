using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithDisabledIsChangedPropertyTests
{
    static TestResult testResult;

    static AssemblyWithDisabledIsChangedPropertyTests()
    {
        var task = new ModuleWeaver
        {
            EnableIsChangedProperty = false
        };
        testResult = task.ExecuteTestRun(
            "AssemblyWithDisabledIsChangedProperty.dll",
            ignoreCodes: ["0x80131869"]);
    }

    [Test]
    public async Task DisabledIsChangedProperty()
    {
        var instance = testResult.GetInstance(nameof(IsChangedClassToTest));
        instance.Property1 = "foo";

        await Assert.That((bool)instance.IsChanged).IsFalse();
    }
}
