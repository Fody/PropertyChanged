using NUnit.Framework;

[TestFixture]
public class ExperimentTests
{
    [Test]
    [Explicit]
    public void Foo()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyExperiments\AssemblyExperiments.csproj");
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
        weaverHelper.Assembly.GetInstance("Experiment");
    }
}