using Mono.Cecil;
using NUnit.Framework;

public class TestAssemblyResolver : DefaultAssemblyResolver
{
    public TestAssemblyResolver()
    {
        AddSearchDirectory(TestContext.CurrentContext.TestDirectory);
    }
}