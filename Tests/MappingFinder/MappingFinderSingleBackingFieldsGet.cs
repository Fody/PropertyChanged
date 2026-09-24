
// ReSharper disable ValueParameterNotUsed
#pragma warning disable 649


public class MappingFinderSingleBackingFieldsGet
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name).IsEqualTo("propertyA");
        await Assert.That(memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name).IsEqualTo("propertyB");
    }

    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        string propertyA;
        public string Property1
        {
            get => propertyA;
            set { }
        }

        string propertyB;
        public string Property2
        {
            get => propertyB;
            set { }
        }
        // ReSharper restore ConvertToAutoProperty
    }
}