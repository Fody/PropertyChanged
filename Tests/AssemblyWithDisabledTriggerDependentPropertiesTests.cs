using Fody;
using Xunit;

public class AssemblyWithDisabledTriggerDependentPropertiesTests
{
    static TestResult testResult;

    static AssemblyWithDisabledTriggerDependentPropertiesTests()
    {
        var weavingTaskFalse = new ModuleWeaver { TriggerDependentProperties = false };
        testResult = weavingTaskFalse.ExecuteTestRun("AssemblyWithDisabledTriggerDependentProperties.dll");
    }

    [Fact]
    public void TriggerDependentPropertiesDisabled()
    {
        var instance = testResult.GetInstance(nameof(DependentPropertiesClassToTest));
        instance.Property1 = "foo";

        Assert.Equal(1, instance.OnProperty1ChangedCallCount);
        Assert.Equal(0, instance.OnProperty2ChangedCallCount);
    }
}
