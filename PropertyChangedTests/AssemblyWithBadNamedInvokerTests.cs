using NUnit.Framework;

[TestFixture]
[Explicit]
public class AssemblyWithBadNamedInvokerTests
{
    [Test]
    public void WithOnNotify()
    {
        var weaverHelper = new WeaverHelper("AssemblyInheritingBadNamedInvoker");
         weaverHelper.Assembly.GetInstance("ChildClass");
        //TODO: validate that a log message is written
        //TODO: move ClassWithForwardedEvent.cs into own project and do the same kind of test
        //EventTester.TestProperty(instance, false);
    }
}
