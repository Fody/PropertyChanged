
// ReSharper disable ValueParameterNotUsed


public class MappingFinderSingleBackingConstantFieldGet
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        var memberMapping = memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1");
        Assert.Null(memberMapping.FieldDefinition);
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