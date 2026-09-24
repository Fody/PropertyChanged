public class MappingFinderLowerCaseBackingFields
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithLowerCaseBackingFields>()).ToList();
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name).IsEqualTo("property1");
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name).IsEqualTo("property2");
    }

    public class ClassWithLowerCaseBackingFields
    {
        // ReSharper disable ConvertToAutoProperty
        string property1;
        public string Property1
        {
            get => property1;
            set => property1 = value;
        }

        string property2;
        public string Property2
        {
            get => property2;
            set => property2 = value;
        }
        // ReSharper restore ConvertToAutoProperty
    }
}