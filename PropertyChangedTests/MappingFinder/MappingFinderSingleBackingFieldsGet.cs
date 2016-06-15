using System.Linq;
using NUnit.Framework;
#pragma warning disable 649

[TestFixture]
public class MappingFinderSingleBackingFieldsGet
{
    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsGet>()).ToList();
        Assert.AreEqual("propertyA", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("propertyB", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }
    public class ClassWithSingleBackingFieldsGet
    {
        // ReSharper disable ConvertToAutoProperty
        string propertyA;
        public string Property1
        {
            get { return propertyA; }
            set { }
        }

        string propertyB;
        public string Property2
        {
            get { return propertyB; }
            set { }
        }
        // ReSharper restore ConvertToAutoProperty
    }
}