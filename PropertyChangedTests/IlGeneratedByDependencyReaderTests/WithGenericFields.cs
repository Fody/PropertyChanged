using System.Linq;
using NUnit.Framework;

[TestFixture]
public class WithGenericFields
{
    [Test]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person<int>>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = MappingFinder.GetMappings(typeDefinition).ToList()
                       };

        new IlGeneratedByDependencyReader(node).Process(); 
        Assert.AreEqual("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.AreEqual("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person<T>
    {
        string givenNames;
        public string GivenNames
        {
            get { return givenNames; }
            set { givenNames = value; }
        }

        string familyName;
        public string FamilyName
        {
            get { return familyName; }
            set { familyName = value; }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", givenNames, familyName);
            }
        }
    }
}