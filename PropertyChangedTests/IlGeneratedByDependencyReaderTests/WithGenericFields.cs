using System.Linq;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global

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
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };

        new IlGeneratedByDependencyReader(node).Process(); 
        Assert.AreEqual("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.AreEqual("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }

        public string FamilyName { get; set; }

        public string FullName => $"{GivenNames} {FamilyName}";
    }
}