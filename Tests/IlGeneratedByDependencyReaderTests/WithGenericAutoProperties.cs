public class WithGenericAutoProperties
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
        var first = node.PropertyDependencies[0];
        await Assert.That(first.ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(first.WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        var second = node.PropertyDependencies[1];
        await Assert.That(second.ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(second.WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}