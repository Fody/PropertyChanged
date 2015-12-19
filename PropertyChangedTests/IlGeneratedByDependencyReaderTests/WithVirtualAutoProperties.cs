using System.Linq;
using NUnit.Framework;

[TestFixture]
public class WithVirtualAutoProperties
{
    //TODO: add test for abstract

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
        var first = node.PropertyDependencies[0];
        Assert.AreEqual(1, node.PropertyDependencies.Count);
        Assert.AreEqual("FullName", first.ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", first.WhenPropertyIsSet.Name);
    }

    public class Person
    {
        public virtual string GivenNames { get; set; }
        public virtual string FullName => GivenNames;
    }
}