using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderLowerCaseBackingFields
{

    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithLowerCaseBackingFields>()).ToList();
        Assert.AreEqual("property1", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("property2", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithLowerCaseBackingFields
    {
        // ReSharper disable ConvertToAutoProperty
        string property1;
        public string Property1
        {
            get { return property1; }
            set { property1 = value; }
        }

        string property2;
        public string Property2
        {
            get { return property2; }
            set { property2 = value; }
        }
        // ReSharper restore ConvertToAutoProperty
    }
}