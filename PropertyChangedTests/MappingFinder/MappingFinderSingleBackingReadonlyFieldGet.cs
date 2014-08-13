using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderSingleBackingReadonlyFieldGet
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
        readonly string propertyA = "foo";
        public string Property1
        {
            get { return propertyA; }
            set { }
        }
    }
}