using System.Linq;
using NUnit.Framework;

[TestFixture]
public class AbstractInfoCheckerTest
{

    [Test]
    public void IsAbstract()
    {
        var checker = new WarningChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>().Properties.First(x => x.Name == "AbstractProperty");

        var message = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                              }, false);
        Assert.IsNotNull(message);
    }

    [Test]
    public void NonAbstract()
    {
        var checker = new WarningChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>().Properties.First(x => x.Name == "NonAbstractProperty");

        var message = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                              }, false);
        Assert.IsNull(message);
    }

    public abstract class BaseClass
    {
        public abstract int AbstractProperty { get; set; }
        public int NonAbstractProperty { get; set; }

    }
}