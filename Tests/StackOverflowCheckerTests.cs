using System.Linq;
using Fody;
using Xunit;

public class StackOverflowCheckerTests
{
    ModuleWeaver stackOverflowChecker = new();

    [Fact]
    public void CanDetectStackOverflow()
    {
        var weavingTask = new ModuleWeaver();
        Assert.Throws<WeavingException>(() => { weavingTask.ExecuteTestRun("AssemblyWithStackOverflow.dll"); });
    }

    [Theory]
    [InlineData("Name", true)]
    [InlineData("ValidName", false)]
    public void CanCheckIfGetterCallsSetter(string propertyName, bool expectedResult)
    {
        var propertyDefinition = DefinitionFinder.FindType<ClassWithStackOverflow>().Properties.First(x => x.Name == propertyName);
        var result = stackOverflowChecker.CheckIfGetterCallsSetter(propertyDefinition);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CanDetectIfGetterCallsVirtualBaseSetter()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildClassWithOverflow>().Properties.First(x => x.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.True(result);
    }

    [Fact]
    public void CanDetectIfGetterCallsVirtualBaseSetterWhenBaseClassInDifferentAssembly()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildWithBaseInDifferentAssembly>().Properties.First(x => x.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.True(result);
    }
}