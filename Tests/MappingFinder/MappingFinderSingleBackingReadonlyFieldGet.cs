using System.Linq;
using Xunit;
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ValueParameterNotUsed


public class MappingFinderSingleBackingReadonlyFieldGet
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        var memberMapping = memberMappings.Single(x => x.PropertyDefinition.Name == "Property1");
        Assert.Null(memberMapping.FieldDefinition);
    }

    public class ClassWithSingleBackingFieldsGet
    {
        readonly string propertyA = "foo";
        public string Property1
        {
            get { return propertyA; }
            set { }
        }
    }
}