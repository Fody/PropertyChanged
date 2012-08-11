using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderClassWithSingleBackingFieldsSet
{
    [Test]
    public void Run()
    {
        var memberMappings = MappingFinder.GetMappings(DefinitionFinder.FindType<ClassWithSingleBackingFieldsSet>()).ToList();
        Assert.AreEqual("propertya", memberMappings.First(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("propertyb", memberMappings.First(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }
    public class ClassWithSingleBackingFieldsSet
    {
        // ReSharper disable ConvertToAutoProperty
// ReSharper disable NotAccessedField.Local
        string propertya;
        public string Property1
        {
            get { return null; }
            set { propertya = value; }
        }

        string propertyb;
        public string Property2
        {
            get { return null; }
            set { propertyb = value; }
        }
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore ConvertToAutoProperty
    }
}