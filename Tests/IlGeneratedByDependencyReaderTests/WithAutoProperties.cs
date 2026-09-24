public class WithAutoProperties
{
    //TODO: add test for abstract

    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
        {
            TypeDefinition = typeDefinition,
            Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
        };
        new IlGeneratedByDependencyReader(node).Process();
        await Assert.That(node.PropertyDependencies.Count).IsEqualTo(2);
        var first = node.PropertyDependencies[0];
        await Assert.That(first.ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(first.WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        var second = node.PropertyDependencies[1];
        await Assert.That(second.ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(second.WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}