#pragma warning disable 649


public class MappingFinderInnerClass
{
    [Fact]
    public void Run()
    {
        var memberMappings = ModuleWeaver.GetMappings(DefinitionFinder.FindType<Model>()).ToList();
        Assert.Null(memberMappings.Single().FieldDefinition);
    }

    public class Model
    {
        InnerClass innerClass;
        public string Property1
        {
            get => innerClass.Property1;
            set => innerClass.Property1 = value;
        }
    }

    class InnerClass
    {
        public string Property1 { get; set; }
    }
}