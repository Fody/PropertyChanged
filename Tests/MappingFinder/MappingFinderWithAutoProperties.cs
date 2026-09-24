public class MappingFinderWithAutoProperties
{
    [Test]
    public async Task Run()
    {

        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithAutoProperties>()).ToList();
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name).IsEqualTo("<Property1>k__BackingField");
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name).IsEqualTo("<Property2>k__BackingField");
    }

    public class ClassWithAutoProperties
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}