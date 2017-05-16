using System.Linq;
using NUnit.Framework;

[TestFixture]
public class StackOverflowCheckerTests
{
    ModuleWeaver stackOverflowChecker;

    [SetUp]
    public void SetUp()
    {
        stackOverflowChecker = new ModuleWeaver();
    }

    [Test]
    public void CanDetectStackOverflow()
    {
        Assert.Throws<WeavingException>(() => new WeaverHelper("AssemblyWithStackOverflow"));
    }

    [TestCase("Name", true)]
    [TestCase("ValidName", false)]
    public void CanCheckIfGetterCallsSetter(string propertyName, bool expectedResult)
    {
        var propertyDefinition = DefinitionFinder.FindType<ClassWithStackOverflow>().Properties.First(x => x.Name == propertyName);
        var result = stackOverflowChecker.CheckIfGetterCallsSetter(propertyDefinition);

        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void CanDetectIfGetterCallsVirtualBaseSetter()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildClass>().Properties.First(x => x.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.AreEqual(true, result);
    }

    [Test]
    public void CanDetectIfGetterCallsVirtualBaseSetterWhenBaseClassInDifferentAssembly()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildWithBaseInDifferentAssembly>().Properties.First(x => x.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.AreEqual(true, result);
    }
}