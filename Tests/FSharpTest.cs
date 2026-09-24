using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class FSharpTest
{
    TestResult testResult;

    public FSharpTest()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyFSharp.dll", runPeVerify: false);
    }

    [Test]
    public void SimpleClass()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithNoOnPropertyChanged()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithNoOnPropertyChanged");
        EventTester.TestProperty(instance, false);
    }
}