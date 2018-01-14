using AssemblyWithBase.BaseWithEquals;
using Fody;
using Xunit;
#pragma warning disable 618

public class AssemblyWithBaseInDifferentModuleTests
{
    TestResult testResult;

    public AssemblyWithBaseInDifferentModuleTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyWithBaseInDifferentModule.dll", ignoreCodes:new []{ "0x80131869" });
    }

    [Fact]
    public void SimpleChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Simple.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericParent.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericFromAbove()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.GenericFromAbove.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void DirectChildClass()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.DirectGeneric.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericChildClassFromMultiType()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.MultiTypes.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericEquals()
    {
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericProperty.Class");
        EventTester.TestProperty(instance, true);
        Assert.True(BaseClass1<int>.EqualsCalled);
    }
}