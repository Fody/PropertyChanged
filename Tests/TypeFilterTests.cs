using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class TypeFilterTests
{
    TestResult testResult;

    public TypeFilterTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun(
            "AssemblyWithTypeFilter.dll",
            ignoreCodes: ["0x80131869"]);
    }

    [Test]
    public void CheckIfFilterTypeExcludeCorrectTypes()
    {
        var instance = testResult.GetInstance("TestClassExclude");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public void CheckIfFilterTypeIncludeCorrectTypes()
    {
        var instance = testResult.GetInstance("PropertyChangedTest.TestClassInclude");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void CheckIfMultipleFilterTypeIncludeCorrectTypes()
    {
        var instance1 = testResult.GetInstance("PropertyChangedTest.TestClassInclude");
        var instance2 = testResult.GetInstance("PropertyChangedTestWithDifferentNamespace.TestClassIncludeAlso");

        EventTester.TestProperty(instance1, false);
        EventTester.TestProperty(instance2, false);
    }
}