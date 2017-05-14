using System.IO;
using Mono.Cecil;
using NUnit.Framework;

public class TestAssemblyResolver : DefaultAssemblyResolver
{
    public TestAssemblyResolver()
    {
        var fullPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\TestAssemblyBin\net462"));
        AddSearchDirectory(fullPath);
    }
}