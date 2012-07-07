using System.Linq;
using NUnit.Framework;

[TestFixture]
public class MappingFinderClassWithInnerClass
{
    [Test]
    public void Run()
    {
        var memberMappings = MappingFinder.GetMappings(DefinitionFinder.FindType<Model>()).ToList();
        Assert.IsNull(memberMappings.First().FieldDefinition);
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