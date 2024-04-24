public class TypeFilterTests
{
    TestResult testResult;

    public TypeFilterTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun(
            "AssemblyWithTypeFilter.dll",
            ignoreCodes: new[] {"0x80131869"});
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