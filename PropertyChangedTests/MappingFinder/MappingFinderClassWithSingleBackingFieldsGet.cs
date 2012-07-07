using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderClassWithSingleBackingFieldsGet
{
    [Test]
    public void Run()
    {
        var memberMappings = MappingFinder.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        Assert.AreEqual("propertya", memberMappings.First(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("propertyb", memberMappings.First(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }
    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        string propertya;
        public string Property1
        {
            get { return propertya; }
            set { }
        }

        string propertyb;
        public string Property2
        {
            get { return propertyb; }
            set { }
        }
        // ReSharper restore ConvertToAutoProperty
    }
}