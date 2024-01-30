public class MappingFinderUnderScoreBackingFields
{
    [Fact]
    public void WithLowerUnderScoreBackingFields()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithUnderScoreBackingFields>()).ToList();
        Assert.Equal("_property1", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.Equal("_property2", memberMappings.Single(_ => _.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithUnderScoreBackingFields
    {
        // ReSharper disable ConvertToAutoProperty
        // ReSharper disable InconsistentNaming
        string _property1;
        public string Property1
        {
            get => _property1;
            set => _property1 = value;
        }

        string _property2;
        public string Property2
        {
            get => _property2;
            set => _property2 = value;
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore ConvertToAutoProperty
    }
}