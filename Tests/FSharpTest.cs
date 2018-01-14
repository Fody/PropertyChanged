using Fody;
using Xunit;
#pragma warning disable 618

public class FSharpTest
{
    TestResult testResult;

    public FSharpTest()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyFSharp.dll"
#if NETCOREAPP2_0
            , runPeVerify: false
#endif
            );
    }

    [Fact]
    public void SimpleClass()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void WithNoOnPropertyChanged()
    {
        var instance = testResult.GetInstance("Namespace.ClassWithNoOnPropertyChanged");
        EventTester.TestProperty(instance, false);
    }
}