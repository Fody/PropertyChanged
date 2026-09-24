using System.Data.SqlTypes;

public class TypeEqualityFinderTests
{
    [Test]
    public async Task TestSqlGuid()
    {
        var typeDefinition = DefinitionFinder.FindType<SqlGuid>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        await Assert.That(findNamedMethod).IsNull();
    }

    [Test]
    public async Task TestInt()
    {
        var typeDefinition = DefinitionFinder.FindType<int>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        await Assert.That(findNamedMethod).IsNull();
    }

    [Test]
    public async Task TestString()
    {
        var typeDefinition = DefinitionFinder.FindType<string>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        await Assert.That(findNamedMethod.FullName).IsEqualTo("System.Boolean System.String::Equals(System.String,System.String)");
    }
}