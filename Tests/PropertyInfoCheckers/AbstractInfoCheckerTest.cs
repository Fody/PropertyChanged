public class AbstractInfoCheckerTest
{
    [Test]
    public async Task IsAbstract()
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
        await Assert.That(message).IsNotNull();
    }

    [Test]
    public async Task NonAbstract()
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
        await Assert.That(message).IsNull();
    }

    public abstract class BaseClass
    {
        public abstract int AbstractProperty { get; set; }
        public int NonAbstractProperty { get; set; }
    }
}