using System.Linq;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

[TestFixture]
public class WithCamelCaseFields
{
    [Test]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        Assert.AreEqual(2, node.PropertyDependencies.Count);
        Assert.AreEqual("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.AreEqual("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person
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

        public string FullName => $"{givenNames} {familyName}";
    }
}