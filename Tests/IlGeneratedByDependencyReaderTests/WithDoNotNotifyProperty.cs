using System.Linq;
using NUnit.Framework;
using PropertyChanged;

[TestFixture]
public class WithDoNotNotifyProperty
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
        Assert.AreEqual(0, node.PropertyDependencies.Count);
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        [DoNotNotify]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}