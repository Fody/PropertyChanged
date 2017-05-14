using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using NUnit.Framework;

public class WeaverHelper
{
    public string BeforeAssemblyPath;
    public string AfterAssemblyPath;
    public Assembly Assembly;

    public WeaverHelper(string assemblyName)
    {
        BeforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\TestAssemblies", assemblyName,@"bin\Debug\net462", assemblyName+".dll"));

#if (RELEASE)
        BeforeAssemblyPath = BeforeAssemblyPath.Replace("Debug", "Release");
#endif
        AfterAssemblyPath = BeforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(BeforeAssemblyPath, AfterAssemblyPath, true);

        using (var moduleDefinition = ModuleDefinition.ReadModule(BeforeAssemblyPath))
        {
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
            };

            weavingTask.Execute();

            moduleDefinition.Write(AfterAssemblyPath);
        }

        Assembly = Assembly.LoadFile(AfterAssemblyPath);
    }

}