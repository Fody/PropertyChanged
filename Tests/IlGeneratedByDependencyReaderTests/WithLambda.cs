public class WithLambda
{
    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<TestClass>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        await Assert.That(node.PropertyDependencies).HasSingleItem();
        await Assert.That(node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name).IsEqualTo("PropertyWithLambda");
        await Assert.That(node.PropertyDependencies[0].WhenPropertyIsSet.Name).IsEqualTo("Property1");
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