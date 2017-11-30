using NUnit.Framework;

[TestFixture]
public class AssemblyUsingAttributeWrongTests
{
    [Test]
    [Explicit]
    public void Foo()
    {
        var assemblyPath = "AssemblyUsingAttributeWrong.dll";
        new WeaverHelper(assemblyPath);

        var assemblyPath2 = assemblyPath.Replace(".dll", "2.dll");
        Verifier.Verify(assemblyPath,assemblyPath2);
    }
}