using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithDisabledInjectOnPropertyNameChangedTests
{
    static TestResult testResult;

    static AssemblyWithDisabledInjectOnPropertyNameChangedTests()
    {
        var task = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = false
        };
        testResult = task.ExecuteTestRun(
            "AssemblyWithDisabledInjectOnPropertyNameChanged.dll",
            ignoreCodes: ["0x80131869"]);
    }

    [Test]
    public async Task DefaultMethodCallsAreNotInjected()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnPropertyChangedMethod));
        instance.Property1 = "foo";

        await Assert.That((int)instance.OnProperty1ChangedCallCount).IsEqualTo(0);
    }

    [Test]
    public async Task CustomMethodCallsAreInjected()
    {
        var instance = testResult.GetInstance(nameof(ClassWithConfiguredOnPropertyChanged));
        instance.Property1 = "foo";

        await Assert.That((int)instance.OnProperty1ChangedCallCount).IsEqualTo(1);
    }
}
