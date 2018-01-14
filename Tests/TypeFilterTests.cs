using Fody;
using Xunit;
#pragma warning disable 618

public class TypeFilterTests
{
    TestResult testResult;

    public TypeFilterTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyWithTypeFilter.dll");
    }

    [Fact]
    public void CheckIfFilterTypeExcludeCorrectTypes()
    {
        var instance = testResult.GetInstance("TestClassExclude");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Fact]
    public void CheckIfFilterTypeIncludeCorrectTypes()
    {
        var instance = testResult.GetInstance("PropertyChangedTest.TestClassInclude");
        EventTester.TestProperty(instance, false);
    }
}