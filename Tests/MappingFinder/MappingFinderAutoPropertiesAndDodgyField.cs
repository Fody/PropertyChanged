public class MappingFinderAutoPropertiesAndDodgyField
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithAutoPropertiesAndDodgyField>()).ToList();
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name).IsEqualTo("<Property1>k__BackingField");
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name).IsEqualTo("<Property2>k__BackingField");
    }

    public class ClassWithAutoPropertiesAndDodgyField
    {
#pragma warning disable 169
        string _property2;
#pragma warning restore 169
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}