using System.Data.SqlTypes;
using NUnit.Framework;

[TestFixture]
public class TypeEqualityFinderTests
{
    [Test]
    public void TestSqlGuid()
    {
        var typeDefinition = DefinitionFinder.FindType<SqlGuid>();
        var findNamedMethod = ModuleWeaver  .FindNamedMethod(typeDefinition);
        Assert.IsNull(findNamedMethod);
    }
    [Test]
    public void TestInt()
    {
        var typeDefinition = DefinitionFinder.FindType<int>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        Assert.IsNull(findNamedMethod);
    }
    [Test]
    public void TestString()
    {
        var typeDefinition = DefinitionFinder.FindType<string>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        Assert.AreEqual("System.Boolean System.String::Equals(System.String,System.String)", findNamedMethod.FullName);
    }
}