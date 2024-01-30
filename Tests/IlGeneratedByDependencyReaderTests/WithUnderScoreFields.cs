

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWhenPossible

public class WithUnderScoreFields
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
        string _givenNames;
        public string GivenNames
        {
            get => _givenNames;
            set => _givenNames = value;
        }

        string _familyName;
        public string FamilyName
        {
            get => _familyName;
            set => _familyName = value;
        }

        public string FullName => $"{_givenNames} {_familyName}";
    }
}