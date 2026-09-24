public class MappingFinderSingleBackingFieldsSet
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsSet>()).ToList();
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name).IsEqualTo("propertyA");
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name).IsEqualTo("propertyB");
    }

    public class ClassWithSingleBackingFieldsSet
    {
        // ReSharper disable ConvertToAutoProperty
// ReSharper disable NotAccessedField.Local
        string propertyA;
        public string Property1
        {
            get => null;
            set => propertyA = value;
        }

        string propertyB;
        public string Property2
        {
            get => null;
            set => propertyB = value;
        }
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore ConvertToAutoProperty
    }
}