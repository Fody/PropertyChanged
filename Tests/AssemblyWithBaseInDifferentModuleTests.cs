using AssemblyWithBase.BaseWithEquals;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class AssemblyWithBaseInDifferentModuleTests :
    XunitLoggingBase
{
    TestResult testResult;

    void Weave(bool useStaticEqualsFromBase)
    {
        var weavingTask = new ModuleWeaver { UseStaticEqualsFromBase = useStaticEqualsFromBase };
        testResult = weavingTask.ExecuteTestRun("AssemblyWithBaseInDifferentModule.dll", ignoreCodes:new []{ "0x80131869" });
    }

    [Fact]
    public void SimpleChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Simple.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericParent.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericFromAbove()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.GenericFromAbove.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void DirectChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.DirectGeneric.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericChildClassFromMultiType()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.MultiTypes.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericProperty.Class");
        EventTester.TestProperty(instance, true);
        Assert.True(BaseClass1<int>.EqualsCalled);
    }

    [Fact]
    public void StaticEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEquals.StaticEquals");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void StaticEquals_Hierarchy()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Hierarchy.ChildClass");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void GenericStaticEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEquals");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void GenericBase_StaticEquals()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEqualsOnBase");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void GenericBase_StaticEquals_BaseNotUsed()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEqualsOnBase");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.False(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void GenericBase_OwnStaticEquals()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.OwnStaticEquals");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.ChildStaticEqualsCalled);
        Assert.False(instance.Property2.StaticEqualsCalled);
        instance.Property2.ChildStaticEqualsCalled = false;
    }

    [Fact]
    public void GenericBase_MultipleBaseClasses__GenericArgsMapping_BaseHasLessArgs()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.ArgsMapping1");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    [Fact]
    public void GenericBase_MultipleBaseClasses_GenericArgsMapping_BaseHasMoreArgs()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.ArgsMapping2");
        EventTester.TestProperty(instance, true);
        Assert.NotNull(instance.Property2);
        Assert.True(instance.Property2.StaticEqualsCalled);
        instance.Property2.StaticEqualsCalled = false;
    }

    public AssemblyWithBaseInDifferentModuleTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}