using System.Linq;
using Xunit;
// ReSharper disable ValueParameterNotUsed


public class IndexerCheckerTest
{
    [Fact]
    public void IsIndexer()
    {
        var checker = new ModuleWeaver();
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>()
            .Properties
            .First();

        var propertyData = new PropertyData
        {
            PropertyDefinition = propertyDefinition,
        };
        var message = checker.CheckForWarning(propertyData, InvokerTypes.String);
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