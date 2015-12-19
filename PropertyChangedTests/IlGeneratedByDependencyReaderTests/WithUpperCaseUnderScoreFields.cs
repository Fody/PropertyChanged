using System.Linq;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable InconsistentNaming

[TestFixture]
public class WithUpperCaseUnderScoreFields
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
        string _GivenNames;
        public string GivenNames
        {
            get { return _GivenNames; }
            set { _GivenNames = value; }
        }

        string _FamilyName;
        public string FamilyName
        {
            get { return _FamilyName; }
            set { _FamilyName = value; }
        }

        public string FullName => $"{_GivenNames} {_FamilyName}";
    }
}