
// ReSharper disable ValueParameterNotUsed


public class MappingFinderSingleBackingConstantFieldGet
{
    [Test]
    public async Task Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        var memberMapping = memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1");
        await Assert.That(memberMapping.FieldDefinition).IsNull();
    }

    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        const string propertyA = "foo";
        public string Property1
        {
            get => propertyA;
            set { }
        }
    }
}