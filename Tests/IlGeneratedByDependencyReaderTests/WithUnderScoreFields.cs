

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWhenPossible

public class WithUnderScoreFields
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