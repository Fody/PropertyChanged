

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable InconsistentNaming


public class WithUpperCaseUnderScoreFields
{
    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<Person>();
        var node = new TypeNode
        {
            TypeDefinition = typeDefinition,
            Mappings = ModuleWeaver.GetMappings(typeDefinition).ToList()
        };
        new IlGeneratedByDependencyReader(node).Process();
        await Assert.That(node.PropertyDependencies.Count).IsEqualTo(2);

        await Assert.That(node.PropertyDependencies[0].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[0].WhenPropertyIsSet.Name).IsEqualTo("GivenNames");
        await Assert.That(node.PropertyDependencies[1].ShouldAlsoNotifyFor.Name).IsEqualTo("FullName");
        await Assert.That(node.PropertyDependencies[1].WhenPropertyIsSet.Name).IsEqualTo("FamilyName");
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