
#if(DEBUG)
using NUnit.Framework;

[TestFixture]
public class FSharpTest
{
    [Test]
    public void EnsureReferenceIsRemoved()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyFSharp\AssemblyFSharp.fsproj");
        var instance = weaverHelper.Assembly.GetInstance("Namespace.ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }

}
#endif