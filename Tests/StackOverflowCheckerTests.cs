// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class StackOverflowCheckerTests
{
    ModuleWeaver stackOverflowChecker = new();

    [Test]
    public async Task CanDetectStackOverflow()
    {
        var weaver = new ModuleWeaver();
        await Assert.That(() => { weaver.ExecuteTestRun("AssemblyWithStackOverflow.dll"); }).Throws<WeavingException>();
    }

    [Test]
    [Arguments("Name", true)]
    [Arguments("ValidName", false)]
    public async Task CanCheckIfGetterCallsSetter(string propertyName, bool expectedResult)
    {
        var propertyDefinition = DefinitionFinder.FindType<ClassWithStackOverflow>().Properties.First(_ => _.Name == propertyName);
        var result = stackOverflowChecker.CheckIfGetterCallsSetter(propertyDefinition);

        await Assert.That(result).IsEqualTo(expectedResult);
    }

    [Test]
    public async Task CanDetectIfGetterCallsVirtualBaseSetter()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildClassWithOverflow>().Properties.First(_ => _.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task CanDetectIfGetterCallsVirtualBaseSetterWhenBaseClassInDifferentAssembly()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildWithBaseInDifferentAssembly>().Properties.First(_ => _.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        await Assert.That(result).IsTrue();
    }
}