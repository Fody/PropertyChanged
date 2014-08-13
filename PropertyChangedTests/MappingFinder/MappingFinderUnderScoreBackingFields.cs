using System.Linq;
using NUnit.Framework;


[TestFixture]
public class MappingFinderUnderScoreBackingFields
{

    [Test]
    public void WithLowerUnderScoreBackingFields()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<ClassWithUnderScoreBackingFields>()).ToList();
        Assert.AreEqual("_property1", memberMappings.Single(x => x.PropertyDefinition.Name == "Property1").FieldDefinition.Name);
        Assert.AreEqual("_property2", memberMappings.Single(x => x.PropertyDefinition.Name == "Property2").FieldDefinition.Name);
    }

  
    
    public class ClassWithUnderScoreBackingFields
    {
        // ReSharper disable ConvertToAutoProperty
        // ReSharper disable InconsistentNaming
        string _property1;
        public string Property1
        {
            get { return _property1; }
            set { _property1 = value; }
        }

        string _property2;
        public string Property2
        {
            get { return _property2; }
            set { _property2 = value; }
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore ConvertToAutoProperty
    }
}