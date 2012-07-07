using System.Linq;
using NUnit.Framework;

[TestFixture]
public class IndexerCheckerTest
{

    [Test]
    public void IsIndexer()
    {
        var checker = new WarningChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>().Properties.First();

        var message = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                              },false);
        Assert.IsNotNull(message);
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