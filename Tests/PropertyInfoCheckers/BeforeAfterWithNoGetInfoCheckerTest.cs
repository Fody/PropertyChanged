
// ReSharper disable UnusedMember.Global


public class BeforeAfterWithNoGetInfoCheckerTest
{
    [Fact]
    public void WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.BeforeAfter);
        Assert.Null(message);
    }

    [Fact]
    public void NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.BeforeAfter);
        Assert.NotNull(message);
    }

    public string PropertyNoGet
    {
        set => PropertyWithGet = value;
    }
    public string PropertyWithGet { set; get; }
}