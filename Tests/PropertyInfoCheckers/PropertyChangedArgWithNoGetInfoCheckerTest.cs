
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible


public class PropertyChangedArgWithNoGetInfoCheckerTest
{
    [Test]
    public async Task WithGet()
    {
        var weaver = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyWithGet");

        var message = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.PropertyChangedArg);
        await Assert.That(message).IsNull();
    }

    [Test]
    public async Task NoGet()
    {
        var weaver = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangedArgWithNoGetInfoCheckerTest>("PropertyNoGet");

        var message = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
            },
            InvokerTypes.PropertyChangedArg);
        await Assert.That(message).IsNotNull();
    }

    string property;

    internal string PropertyNoGet
    {
        set => property = value;
    }
    public string PropertyWithGet
    {
        set => property = value;
        get => property;
    }
}