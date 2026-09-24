
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ValueParameterNotUsed


public class MappingFinderSingleBackingReadonlyFieldGet
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
        readonly string propertyA = "foo";
        public string Property1
        {
            get => propertyA;
            set { }
        }
    }
}