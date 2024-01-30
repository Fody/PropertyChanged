public class MappingFinderLowerCaseBackingFields
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithLowerCaseBackingFields>()).ToList();
        Assert.Equal("property1", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.Equal("property2", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
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