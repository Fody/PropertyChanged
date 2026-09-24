
// ReSharper disable ValueParameterNotUsed

public class IndexerCheckerTest
{
    [Test]
    public async Task IsIndexer()
    {
        var weaver = new ModuleWeaver();
        var propertyDefinitions = DefinitionFinder.FindType<IndexerClass>()
            .Properties;

        foreach (var propertyDefinition in propertyDefinitions)
        {
            var propertyData = new PropertyData
            {
                PropertyDefinition = propertyDefinition,
            };

            var message = weaver.CheckForWarning(propertyData, InvokerTypes.String);
            await Assert.That(message).IsEqualTo("Property is an indexer.");
        }
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

        public double this[int x, int y, int z]
        {
            get => 0.0;
            set
            {
            }
        }
    }
}