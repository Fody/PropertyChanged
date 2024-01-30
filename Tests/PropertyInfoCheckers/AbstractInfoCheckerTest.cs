public class AbstractInfoCheckerTest
{
    [Fact]
    public void IsAbstract()
    {
        var weaver = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>()
            .Properties
            .First(_ => _.Name == "AbstractProperty");

        var message = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.String);
        Assert.NotNull(message);
    }

    [Fact]
    public void NonAbstract()
    {
        var weaver = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>()
            .Properties
            .First(_ => _.Name == "NonAbstractProperty");

        var message = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.String);
        Assert.Null(message);
    }

    public abstract class BaseClass
    {
        public abstract int AbstractProperty { get; set; }
        public int NonAbstractProperty { get; set; }
    }
}