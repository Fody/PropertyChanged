using System.Linq;
using Xunit;

public class WithOverrideAbstractAutoProperties
{
    [Fact]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Derived>();
        var node = new TypeNode
                       {
                           TypeDefinition = typeDefinition,
                           Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
                       };
        new IlGeneratedByDependencyReader(node).Process();
        var first = node.PropertyDependencies[0];
        Assert.Single(node.PropertyDependencies);
        Assert.Equal("Bar", first.ShouldAlsoNotifyFor.Name);
        Assert.Equal("Foo", first.WhenPropertyIsSet.Name);
    }

    public abstract class Base
    {
        public abstract bool Foo { get; set; }
    }

    public class Derived : Base
    {
        public override bool Foo { get; set; }
        public bool Bar => !Foo;
    }
}
