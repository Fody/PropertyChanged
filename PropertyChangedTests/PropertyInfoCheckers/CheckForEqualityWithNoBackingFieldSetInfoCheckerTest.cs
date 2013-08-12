using NUnit.Framework;

[TestFixture]
public class CheckForEqualityWithNoBackingFieldSetInfoCheckerTest
{

    [Test]
    public void WithBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty(() => WithBackingFieldProperty);

        var warning = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  BackingFieldReference = propertyDefinition.DeclaringType.Fields[0]
                                                              }, InvokerTypes.String);
        Assert.IsNull(warning);
    }

    [Test]
    public void WithoutBackingField()
    {
        var checker = new ModuleWeaver();

        var propertyDefinition = DefinitionFinder.FindProperty<CheckForEqualityWithNoBackingFieldSetInfoCheckerTest>("WithoutBackingFieldProperty");

        var warning = checker.CheckForWarning(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  BackingFieldReference = null,
                                                              }, InvokerTypes.String);
        Assert.IsNotNull(warning);
    }


    public int WithBackingFieldProperty { get; set; }

    public int WithoutBackingFieldProperty
    {
        set
        {
        }
    }

}