using NSubstitute;
using NUnit.Framework;
using PropertyChanged;
// ReSharper disable UnusedVariable


[TestFixture]
public class DependsOnDataAttributeReaderTests
{

    [Test]
    public void Integration()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
                       {
                           TypeDefinition = DefinitionFinder.FindType<Person>()
                       };
        reader.ProcessDependsOnAttributes(node);

        var dependencies = node.PropertyDependencies;
        Assert.AreEqual("FullName", dependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", dependencies[0].WhenPropertyIsSet.Name);
        Assert.AreEqual("FullName", dependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("FamilyName", dependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person
    {

        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [DependsOn("GivenNames", "FamilyName")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    [Test]
    public void PropertyThatDoesNotExist()
    {
        var logger = Substitute.For<ModuleWeaver>();
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

}