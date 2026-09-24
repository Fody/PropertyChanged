using PropertyChanged;

public class WithDoNotNotifyProperty
{
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
        await Assert.That(node.PropertyDependencies).IsEmpty();
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        [DoNotNotify]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}