using System.Data.SqlTypes;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class TypeEqualityFinderTests :
    VerifyBase
{
    [Fact]
    public void TestSqlGuid()
    {
        var typeDefinition = DefinitionFinder.FindType<SqlGuid>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        Assert.Null(findNamedMethod);
    }

    [Fact]
    public void TestInt()
    {
        var typeDefinition = DefinitionFinder.FindType<int>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        Assert.Null(findNamedMethod);
    }

    [Fact]
    public void TestString()
    {
        var typeDefinition = DefinitionFinder.FindType<string>();
        var findNamedMethod = ModuleWeaver.FindNamedMethod(typeDefinition);
        Assert.Equal("System.Boolean System.String::Equals(System.String,System.String)", findNamedMethod.FullName);
    }

    public TypeEqualityFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}