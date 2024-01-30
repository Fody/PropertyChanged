

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable InconsistentNaming


public class WithUpperCaseUnderScoreFields
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
        Assert.Equal(2, node.PropertyDependencies.Count);

        Assert.Equal("FullName", node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name);
        Assert.Equal("GivenNames", node.PropertyDependencies[0].WhenPropertyIsSet.Name);
        Assert.Equal("FullName", node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name);
        Assert.Equal("FamilyName", node.PropertyDependencies[1].WhenPropertyIsSet.Name);
    }

    public class Person
    {
        string _GivenNames;
        public string GivenNames
        {
            get => _GivenNames;
            set => _GivenNames = value;
        }

        string _FamilyName;
        public string FamilyName
        {
            get => _FamilyName;
            set => _FamilyName = value;
        }

        public string FullName => $"{_GivenNames} {_FamilyName}";
    }
}