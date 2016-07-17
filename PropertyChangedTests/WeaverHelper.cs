using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using NUnit.Framework;

public class WeaverHelper
{
    string projectPath;
    public string BeforeAssemblyPath;
    public string AfterAssemblyPath;
    public Assembly Assembly;

    public WeaverHelper(string projectPath)
    {
        this.projectPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestAssemblies", projectPath));

        GetAssemblyPath();


        AfterAssemblyPath = BeforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(BeforeAssemblyPath, AfterAssemblyPath, true);


        var assemblyResolver = new TestAssemblyResolver(BeforeAssemblyPath, this.projectPath);
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver
        };
        var moduleDefinition = ModuleDefinition.ReadModule(AfterAssemblyPath, readerParameters);
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = assemblyResolver
                              };

        weavingTask.Execute();

        moduleDefinition.Write(AfterAssemblyPath);

        Assembly = Assembly.LoadFile(AfterAssemblyPath);
    }

    void GetAssemblyPath()
    {
        BeforeAssemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), GetOutputPathValue(), GetAssemblyName() + ".dll");
    }

    string GetAssemblyName()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        return xDocument.Descendants("AssemblyName")
            .Select(x => x.Value)
            .First();
    }

    string GetOutputPathValue()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        var outputPathValue = (from propertyGroup in xDocument.Descendants("PropertyGroup")
                               let condition = (string)propertyGroup.Attribute("Condition")
                               where (condition != null) &&
                                     (condition.Trim() == "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
                               from outputPath in propertyGroup.Descendants("OutputPath")
                               select outputPath.Value).First();
#if (!DEBUG)
            outputPathValue = outputPathValue.Replace("Debug", "Release");
#endif
        return outputPathValue;
    }


}