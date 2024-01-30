public class WithLambda
{
    [Fact]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<TestClass>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        Assert.Single(node.PropertyDependencies);
        Assert.Equal("PropertyWithLambda", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.Equal("Property1", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
    }

    public class TestClass
    {
        public double PropertyWithLambda
        {
            get
            {
                var dashArray = new[] { 5D }.Select(a => a / Property1);
                return dashArray.First();
            }
        }

        public double Property1 { get; set; }
    }
}