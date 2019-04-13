using PropertyChanged;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UnusedVariable
public class DependsOnDataAttributeReaderTests :
    XunitLoggingBase
{
    [Fact]
    public void Integration()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<Person>()
        };
        reader.ProcessDependsOnAttributes(node);

        var dependencies = node.PropertyDependencies;
        Assert.Equal("FullName", dependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", dependencies[0].WhenPropertyIsSet.Name);
        Assert.Equal("FullName", dependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.Equal("FamilyName", dependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [DependsOn("GivenNames", "FamilyName")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    [Fact]
    public void PropertyThatDoesNotExist()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<ClassWithInvalidDepends>(),
        };
        reader.ProcessDependsOnAttributes(node);
        //TODO: should raise an exception
        //logger.Received().LogError("Could not find property 'NotAProperty2' for DependsOnAttribute assigned to 'FullName'.");
        //logger.Received().LogError("Could not find property 'NotAProperty1' for DependsOnAttribute assigned to 'FullName'.");
    }

    public class ClassWithInvalidDepends
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [DependsOn("NotAProperty1", "NotAProperty2")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    public DependsOnDataAttributeReaderTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}