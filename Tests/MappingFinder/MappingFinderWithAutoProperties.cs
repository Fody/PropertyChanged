public class MappingFinderWithAutoProperties
{
    [Fact]
    public void Run()
    {

        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithAutoProperties>()).ToList();
        Assert.Equal("<Property1>k__BackingField", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.Equal("<Property2>k__BackingField", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithAutoProperties
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}