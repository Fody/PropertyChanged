using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderWithAutoProperties
{

    [Test]
    public void Run()
    {

        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithAutoProperties>()).ToList();
        Assert.AreEqual("<Property1>k__BackingField", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("<Property2>k__BackingField", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithAutoProperties
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}