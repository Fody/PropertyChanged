using System.Linq;
using NUnit.Framework;
// ReSharper disable ValueParameterNotUsed

[TestFixture]
public class IndexerCheckerTest
{
    [Test]
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
        Assert.AreEqual("Property is an indexer.", message);
    }

    public abstract class IndexerClass
    {
        public string this[string i]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}