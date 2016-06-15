using System.Linq;
using NUnit.Framework;
#pragma warning disable 649

[TestFixture]
public class MappingFinderInnerClass
{
    [Test]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<Model>()).ToList();
        Assert.IsNull(memberMappings.Single().FieldDefinition);
    }

    public class Model
    {
        InnerClass innerClass;
        public string Property1
        {
            get { return innerClass.Property1; }
            set { innerClass.Property1 = value; }
        }
    }

    class InnerClass
    {
        public string Property1 { get; set; }
    }
}