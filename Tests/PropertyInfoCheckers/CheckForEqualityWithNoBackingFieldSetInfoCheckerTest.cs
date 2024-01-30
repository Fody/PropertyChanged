
// ReSharper disable ValueParameterNotUsed


public class CheckForEqualityWithNoBackingFieldSetInfoCheckerTest
{
    [Fact]
    public void WithBackingField()
    {
        var weaver = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty(() => WithBackingFieldProperty);

        var warning = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
                BackingFieldReference = propertyDefinition.DeclaringType.Fields[0]
            },
            InvokerTypes.String);
        Assert.Null(warning);
    }

    [Fact]
    public void WithoutBackingField()
    {
        var weaver = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<CheckForEqualityWithNoBackingFieldSetInfoCheckerTest>("WithoutBackingFieldProperty");

        var warning = weaver.CheckForWarning(
            new()
            {
                PropertyDefinition = propertyDefinition,
                BackingFieldReference = null,
            },
            InvokerTypes.String);
        Assert.NotNull(warning);
    }

    public int WithBackingFieldProperty { get; set; }

    public int WithoutBackingFieldProperty
    {
        set
        {
        }
    }
}