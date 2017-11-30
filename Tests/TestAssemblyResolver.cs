using Mono.Cecil;
using NUnit.Framework;

public class TestAssemblyResolver : DefaultAssemblyResolver
{
    public TestAssemblyResolver()
    {
        AddSearchDirectory(TestContext.CurrentContext.TestDirectory);
    }

    public override AssemblyDefinition Resolve(AssemblyNameReference name)
    {
#if (NETCOREAPP2_0)
        if (name.Name == "netstandard")
        {
            var netstandard = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
                @"dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.0\build\netstandard2.0\ref\netstandard.dll");
            return AssemblyDefinition.ReadAssembly(
                fileName: netstandard,
                parameters: new ReaderParameters(ReadingMode.Deferred)
                {
                    AssemblyResolver = this
                });
        }
#endif

        return base.Resolve(name);
    }

}