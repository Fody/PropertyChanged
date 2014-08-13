using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderSingleBackingConstantFieldGet
{
    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        var memberMapping = memberMappings.Single(x => x.PropertyDefinition.Name == "Property1");
        Assert.IsNull(memberMapping.FieldDefinition);
    }
    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        const string propertyA = "foo";
        public string Property1
        {
            get { return propertyA; }
            set { }
        }
    }
}