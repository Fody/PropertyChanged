

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

public class WithCamelCaseFields
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
        string givenNames;
        public string GivenNames
        {
            get => givenNames;
            set => givenNames = value;
        }

        string familyName;
        public string FamilyName
        {
            get => familyName;
            set => familyName = value;
        }

        public string FullName => $"{givenNames} {familyName}";
    }
}