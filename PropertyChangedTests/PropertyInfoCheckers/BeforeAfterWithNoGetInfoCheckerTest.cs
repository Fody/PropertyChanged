using NUnit.Framework;
// ReSharper disable UnusedMember.Global

[TestFixture]
public class BeforeAfterWithNoGetInfoCheckerTest
{

    [Test]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.BeforeAfter);
        Assert.IsNull(message);
    }

    [Test]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.BeforeAfter);
        Assert.IsNotNull(message);
    }


    public string PropertyNoGet
    {
        set { PropertyWithGet = value; }
    }
    public string PropertyWithGet { set; get; }
}