
// ReSharper disable ValueParameterNotUsed

public class IndexerCheckerTest
{
    [Fact]
    public void IsIndexer()
    {
        var weaver = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>()
            .Properties
            .First();

        var propertyData = new PropertyData
        {
            PropertyDefinition = propertyDefinition,
        };
        var message = weaver.CheckForWarning(propertyData, InvokerTypes.String);
        Assert.Equal("Property is an indexer.", message);
    }

    public abstract class IndexerClass
    {
        public string this[string i]
        {
            get => null;
            set
            {
            }
        }
    }
}