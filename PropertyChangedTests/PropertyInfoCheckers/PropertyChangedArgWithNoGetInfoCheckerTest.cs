using NUnit.Framework;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

[TestFixture]
public class PropertyChangedArgWithNoGetInfoCheckerTest
{

    [Test]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangedArg);
        Assert.IsNull(message);
    }

    [Test]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangedArg);
        Assert.IsNotNull(message);
    }




    string property;

    public string PropertyNoGet
    {
        set { property = value; }
    }
    public string PropertyWithGet
    {
        set { property = value; }
        get { return property; }
    }

}