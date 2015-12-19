using System.Linq;
using NUnit.Framework;

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
        // ReSharper disable InconsistentNaming
        string _GivenNames;
        // ReSharper restore InconsistentNaming
        public string GivenNames
        {
            get { return _GivenNames; }
            set { _GivenNames = value; }
        }

        // ReSharper disable InconsistentNaming
        string _FamilyName;
        // ReSharper restore InconsistentNaming
        public string FamilyName
        {
            get { return _FamilyName; }
            set { _FamilyName = value; }
        }

        public string FullName => $"{_GivenNames} {_FamilyName}";
    }
}