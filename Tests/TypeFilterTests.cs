using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class TypeFilterTests :
    VerifyBase
{
    TestResult testResult;

    public TypeFilterTests(ITestOutputHelper output) :
        base(output)
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

    [Fact]
    public void CheckIfMultipleFilterTypeIncludeCorrectTypes()
    {
        var instance1 = testResult.GetInstance("PropertyChangedTest.TestClassInclude");
        var instance2 = testResult.GetInstance("PropertyChangedTestWithDifferentNamespace.TestClassIncludeAlso");

        EventTester.TestProperty(instance1, false);
        EventTester.TestProperty(instance2, false);
    }
}