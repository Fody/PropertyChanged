public class WithGenericAutoProperties
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
        var first = node.PropertyDependencies[0];
        Assert.Equal("FullName", first.ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", first.WhenPropertyIsSet.Name);
        var second = node.PropertyDependencies[1];
        Assert.Equal("FullName", second.ShouldAlsoNotifyFor.Name);
        Assert.Equal("FamilyName", second.WhenPropertyIsSet.Name);
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}