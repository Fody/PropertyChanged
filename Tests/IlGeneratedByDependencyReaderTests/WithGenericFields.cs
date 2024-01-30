

// ReSharper disable UnusedMember.Global
public class WithGenericFields
{
    [Fact]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person<int>>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };

        new IlGeneratedByDependencyReader(node).Process();
        Assert.Equal("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.Equal("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.Equal("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }

        public string FamilyName { get; set; }

        public string FullName => $"{GivenNames} {FamilyName}";
    }
}