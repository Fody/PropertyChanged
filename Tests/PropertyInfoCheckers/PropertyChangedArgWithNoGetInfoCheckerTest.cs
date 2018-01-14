using Xunit;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible


public class PropertyChangedArgWithNoGetInfoCheckerTest
{
    [Fact]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangedArg);
        Assert.Null(message);
    }

    [Fact]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                }, InvokerTypes.PropertyChangedArg);
        Assert.NotNull(message);
    }

    string property;

    public string PropertyNoGet
    {
        set => property = value;
    }
    public string PropertyWithGet
    {
        set => property = value;
        get => property;
    }
}