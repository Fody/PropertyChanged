
// ReSharper disable UnusedMember.Global


public class BeforeAfterWithNoGetInfoCheckerTest
{
    [Test]
    public async Task WithGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.BeforeAfter);
        await Assert.That(message).IsNull();
    }

    [Test]
    public async Task NoGet()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = checker.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.BeforeAfter);
        await Assert.That(message).IsNotNull();
    }

    internal string PropertyNoGet
    {
        set => PropertyWithGet = value;
    }
    public string PropertyWithGet { set; get; }
}