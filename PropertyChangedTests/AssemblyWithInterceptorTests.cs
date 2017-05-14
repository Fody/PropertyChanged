using System.Reflection;
using NUnit.Framework;


[TestFixture]
public class AssemblyWithInterceptorTests
{
    [Test]
    public void Simple()
    {
        var weaverHelper = new WeaverHelper("AssemblyWithInterceptor");

        var assembly = weaverHelper.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.IsTrue(value);
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);

    }
    [Test]
    public void BeforeAfter()
    {
        var weaverHelper = new WeaverHelper("AssemblyWithBeforeAfterInterceptor");
        var assembly = weaverHelper.Assembly;
        var instance = assembly.GetInstance("ClassToTest");
        EventTester.TestProperty(instance, false);
        var type = assembly.GetType("PropertyChangedNotificationInterceptor");
        var propertyInfo = type.GetProperty("InterceptCalled", BindingFlags.Static | BindingFlags.Public);
        var value = (bool)propertyInfo.GetValue(null, null);
        Assert.IsTrue(value);
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
    }
}