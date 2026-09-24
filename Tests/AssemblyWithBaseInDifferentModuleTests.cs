using AssemblyWithBase.BaseWithEquals;
using Mono.Cecil;
using Mono.Cecil.Cil;

using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithBaseInDifferentModuleTests
{
    TestResult testResult;

    void Weave(bool useStaticEqualsFromBase)
    {
        var weavingTask = new ModuleWeaver
        {
            UseStaticEqualsFromBase = useStaticEqualsFromBase
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyWithBaseInDifferentModule.dll", ignoreCodes: ["0x80131869"]);
    }

    [Test]
    public void SimpleChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Simple.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericParent.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericFromAbove()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.GenericFromAbove.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void DirectChildClass()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.DirectGeneric.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildClassFromMultiType()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.MultiTypes.ChildClass");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.BaseWithGenericProperty.Class");
        EventTester.TestProperty(instance, true);
        await Assert.That(BaseClass1<int>.EqualsCalled).IsTrue();
    }

    [Test]
    public async Task StaticEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEquals.StaticEquals");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task StaticEquals_Hierarchy()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.Hierarchy.ChildClass");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericStaticEquals()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEquals");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericBase_StaticEquals()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEqualsOnBase");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericBase_StaticEquals_BaseNotUsed()
    {
        Weave(false);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.StaticEqualsOnBase");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsFalse();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericBase_OwnStaticEquals()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.OwnStaticEquals");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.ChildStaticEqualsCalled).IsTrue();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsFalse();
        instance.Property2.ChildStaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericBase_MultipleBaseClasses__GenericArgsMapping_BaseHasLessArgs()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.ArgsMapping1");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task GenericBase_MultipleBaseClasses_GenericArgsMapping_BaseHasMoreArgs()
    {
        Weave(true);
        var instance = testResult.GetInstance("AssemblyWithBaseInDifferentModule.StaticEqualsGenericParent.ArgsMapping2");
        EventTester.TestProperty(instance, true);
        await Assert.That((object)instance.Property2).IsNotNull();
        await Assert.That((bool)instance.Property2.StaticEqualsCalled).IsTrue();
        instance.Property2.StaticEqualsCalled = false;
    }

    [Test]
    public async Task ClassWithGenericTypeInInheritanceChainUsesCorrectEventInvoker()
    {
        // Issue #477

        Weave(false);

        using (var module = ModuleDefinition.ReadModule(testResult.AssemblyPath))
        {
            var typeDef = module.GetType(nameof(ClassWithGenericMiddleChildInDifferentModule));
            var setter = typeDef.Methods.Single(m => m.Name == "set_" + nameof(ClassWithGenericMiddleChildInDifferentModule.Property));
            var callInstruction = setter.Body.Instructions.Single(i => i.OpCode == OpCodes.Callvirt);
            await Assert.That(((MethodReference)callInstruction.Operand).DeclaringType.FullName).IsEqualTo(nameof(BaseClassWithGenericMiddleBase));
        }

        var instance = testResult.GetInstance(nameof(ClassWithGenericMiddleChildInDifferentModule));
        EventTester.TestProperty(instance, nameof(ClassWithGenericMiddleChildInDifferentModule.Property), 42);
    }
}
