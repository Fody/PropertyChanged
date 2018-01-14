using Xunit;
// ReSharper disable ValueParameterNotUsed


public class CheckForEqualityWithNoBackingFieldSetInfoCheckerTest
{
    [Fact]
    public void WithBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty(() => WithBackingFieldProperty);

        var warning = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  BackingFieldReference = propertyDefinition.DeclaringType.Fields[0]
                                                              }, InvokerTypes.String);
        Assert.Null(warning);
    }

    [Fact]
    public void WithoutBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<CheckForEqualityWithNoBackingFieldSetInfoCheckerTest>("WithoutBackingFieldProperty");

        var warning = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  BackingFieldReference = null,
                                                              }, InvokerTypes.String);
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