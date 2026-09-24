

// ReSharper disable UnusedMember.Global
public class WithGenericFields
{
    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person<int>>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };

        new IlGeneratedByDependencyReader(node).Process();
        await Assert.That(node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[0].WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        await Assert.That(node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[1].WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }

        public string FamilyName { get; set; }

        public string FullName => $"{GivenNames} {FamilyName}";
    }
}