using System.Linq;
using NUnit.Framework;


[TestFixture]
public class WithGenericAutoProperties
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
        var first = node.PropertyDependencies[0];
        Assert.AreEqual("FullName", first.ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("GivenNames", first.WhenPropertyIsSet.Name);
        var second = node.PropertyDependencies[1];
        Assert.AreEqual("FullName", second.ShouldAlsoNotifyFor.Name);
        Assert.AreEqual("FamilyName", second.WhenPropertyIsSet.Name);
    }

    public class Person<T>
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}