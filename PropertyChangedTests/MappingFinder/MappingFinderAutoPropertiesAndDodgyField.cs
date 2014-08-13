using System.Linq;
using NUnit.Framework;


[TestFixture]
public class MappingFinderAutoPropertiesAndDodgyField
{

    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithAutoPropertiesAndDodgyField>()).ToList();
        Assert.AreEqual("<Property1>k__BackingField", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("<Property2>k__BackingField", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

    public class ClassWithAutoPropertiesAndDodgyField
    {
#pragma warning disable 169
        string _property2;
#pragma warning restore 169
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}