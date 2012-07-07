using NUnit.Framework;

[TestFixture]
public class AssemblyUsingAttributeWrongTests
{
    [Test]
    [Ignore]
    public void Foo()
    {
        var assemblyPath = @"..\..\..\AssemblyUsingAttributeWrong\bin\Debug\AssemblyUsingAttributeWrong.dll";
        new WeaverHelper(@"AssemblyUsingAttributeWrong\AssemblyUsingAttributeWrong.csproj");

        var assemblyPath2 = assemblyPath.Replace(".dll", "2.dll");
        Verifier.Verify(assemblyPath2);
    }
}