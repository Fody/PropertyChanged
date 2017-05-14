using NUnit.Framework;

[TestFixture]
public class TypeFilterTests
{
    [Test]
    public void CheckIfFilterTypeExcludeRightTypes()
    {
        var weaverHelper = new WeaverHelper("AssemblyWithTypeFilter");

        var assembly = weaverHelper.Assembly;
        var instance = assembly.GetInstance("TestClassExclude");
        EventTester.TestPropertyNotCalled(instance);
        instance = assembly.GetInstance("PropertyChangedTest.TestClassInclude");
        EventTester.TestProperty(instance, false);
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
    }
}
