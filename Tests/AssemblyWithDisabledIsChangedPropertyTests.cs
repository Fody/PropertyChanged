using Fody;
using Xunit;

public class AssemblyWithDisabledIsChangedPropertyTests
{
    static TestResult testResult;

    static AssemblyWithDisabledIsChangedPropertyTests()
    {
        var weavingTaskFalse = new ModuleWeaver
        {
            EnableIsChangedProperty = false
        };
        testResult = weavingTaskFalse.ExecuteTestRun(
            "AssemblyWithDisabledIsChangedProperty.dll",
            ignoreCodes: new[] {"0x80131869"});
    }

    [Fact]
    public void DisabledIsChangedProperty()
    {
        var instance = testResult.GetInstance(nameof(IsChangedClassToTest));
        instance.Property1 = "foo";

        Assert.True(instance.IsChanged != true);
    }
}
