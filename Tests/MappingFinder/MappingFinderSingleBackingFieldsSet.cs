public class MappingFinderSingleBackingFieldsSet
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsSet>()).ToList();
        Assert.Equal("propertyA", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.Equal("propertyB", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
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