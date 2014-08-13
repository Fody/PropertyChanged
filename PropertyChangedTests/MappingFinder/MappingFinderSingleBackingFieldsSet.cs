using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderSingleBackingFieldsSet
{
    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsSet>()).ToList();
        Assert.AreEqual("propertyA", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("propertyB", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }
    public class ClassWithSingleBackingFieldsSet
    {
        // ReSharper disable ConvertToAutoProperty
// ReSharper disable NotAccessedField.Local
        string propertyA;
        public string Property1
        {
            get { return null; }
            set { propertyA = value; }
        }

        string propertyB;
        public string Property2
        {
            get { return null; }
            set { propertyB = value; }
        }
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore ConvertToAutoProperty
    }
}