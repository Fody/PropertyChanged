public class StackOverflowCheckerTests
{
    ModuleWeaver stackOverflowChecker = new();

    [Fact]
    public void CanDetectStackOverflow()
    {
        var weaver = new ModuleWeaver();
        Assert.Throws<WeavingException>(() => { weaver.ExecuteTestRun("AssemblyWithStackOverflow.dll"); });
    }

    [Theory]
    [InlineData("Name", true)]
    [InlineData("ValidName", false)]
    public void CanCheckIfGetterCallsSetter(string propertyName, bool expectedResult)
    {
        var propertyDefinition = DefinitionFinder.FindType<ClassWithStackOverflow>().Properties.First(_ => _.Name == propertyName);
        var result = stackOverflowChecker.CheckIfGetterCallsSetter(propertyDefinition);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CanDetectIfGetterCallsVirtualBaseSetter()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildClassWithOverflow>().Properties.First(_ => _.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.True(result);
    }

    [Fact]
    public void CanDetectIfGetterCallsVirtualBaseSetterWhenBaseClassInDifferentAssembly()
    {
        var propertyDefinition = DefinitionFinder.FindType<ChildWithBaseInDifferentAssembly>().Properties.First(_ => _.Name == "Property1");
        var result = stackOverflowChecker.CheckIfGetterCallsVirtualBaseSetter(propertyDefinition);

        Assert.True(result);
    }
}