using System.Linq;
using ApprovalTests;
using NUnit.Framework;

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
        Approvals.Verify(message);
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