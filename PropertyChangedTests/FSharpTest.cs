
#if(DEBUG)
using NUnit.Framework;

[TestFixture]
public class FSharpTest
{
    WeaverHelper weaverHelper;

    public FSharpTest()
    {
        weaverHelper = new WeaverHelper("AssemblyFSharp");
    }

    [Test]
    public void EnsureReferenceIsRemoved()
    {
        var instance = weaverHelper.Assembly.GetInstance("Namespace.ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void Verify()
    {
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
    }

}
#endif