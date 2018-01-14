using System.Linq;
using PropertyChanged;
using Xunit;

public class WithDoNotNotifyProperty
{
    [Fact]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
        {
            TypeDefinition = typeDefinition,
            Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
        };
        new IlGeneratedByDependencyReader(node).Process();
        Assert.Empty(node.PropertyDependencies);
    }

    public class Person
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        [DoNotNotify]
        public string FullName => $"{GivenNames} {FamilyName}";
    }
}