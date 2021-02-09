using Fody;
using Xunit;

public class AssemblyWithDisabledInjectOnPropertyNameChangedTests
{
    static TestResult testResult;

    static AssemblyWithDisabledInjectOnPropertyNameChangedTests()
    {
        var weavingTask = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = false
        };
        testResult = weavingTask.ExecuteTestRun(
            "AssemblyWithDisabledInjectOnPropertyNameChanged.dll",
            ignoreCodes: new[] {"0x80131869"});
    }

    [Fact]
    public void DefaultMethodCallsAreNotInjected()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnPropertyChangedMethod));
        instance.Property1 = "foo";

        Assert.Equal(0, instance.OnProperty1ChangedCallCount);
    }

    [Fact]
    public void CustomMethodCallsAreInjected()
    {
        var instance = testResult.GetInstance(nameof(ClassWithConfiguredOnPropertyChanged));
        instance.Property1 = "foo";

        Assert.Equal(1, instance.OnProperty1ChangedCallCount);
    }
}
