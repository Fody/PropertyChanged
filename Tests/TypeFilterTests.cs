using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class TypeFilterTests
{
    WeaverHelper weaverHelper;

    public TypeFilterTests()
    {
        weaverHelper = new WeaverHelper("AssemblyWithTypeFilter");
    }

    [Test]
    public void CheckIfFilterTypeExcludeCorrectTypes()
    {
        var instance = weaverHelper.Assembly.GetInstance("TestClassExclude");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public void CheckIfFilterTypeIncludeCorrectTypes()
    {
        var instance = weaverHelper.Assembly.GetInstance("PropertyChangedTest.TestClassInclude");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void Verify()
    {
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
    }
}