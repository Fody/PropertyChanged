using PropertyChanged;

// ReSharper disable UnusedVariable
public class DependsOnDataAttributeReaderTests
{
    [Test]
    public async Task Integration()
    {
        var reader = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<Person>()
        };
        reader.ProcessDependsOnAttributes(node);

        var dependencies = node.PropertyDependencies;
        await Assert.That(dependencies[0].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(dependencies[0].WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        await Assert.That(dependencies[1].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(dependencies[1].WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [PropertyChanged.DependsOn("GivenNames", "FamilyName")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }

    [Test]
    public void PropertyThatDoesNotExist()
    {
        var weaver = new ModuleWeaver();
        var node = new TypeNode
        {
            TypeDefinition = DefinitionFinder.FindType<ClassWithInvalidDepends>(),
        };
        weaver.ProcessDependsOnAttributes(node);
        //TODO: should raise an exception
        //logger.Received().LogError("Could not find property 'NotAProperty2' for DependsOnAttribute assigned to 'FullName'.");
        //logger.Received().LogError("Could not find property 'NotAProperty1' for DependsOnAttribute assigned to 'FullName'.");
    }

    public class ClassWithInvalidDepends
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        [PropertyChanged.DependsOn("NotAProperty1", "NotAProperty2")]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}