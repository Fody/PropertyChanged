using NUnit.Framework;

[TestFixture]
public class AssemblyWithBadNamedInvokerTests
{
    [Test]
    public void Run()
    {
        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            new WeaverHelper("AssemblyInheritingBadNamedInvoker");
        });
        Assert.AreEqual("Could not inject EventInvoker method on type 'ChildClass'. It is possible you are inheriting from a base class and have not correctly set 'EventInvokerNames' or you are using a explicit PropertyChanged event and the event field is not visible to this instance. Either correct 'EventInvokerNames' or implement your own EventInvoker on this class. If you want to suppress this place a [DoNotNotifyAttribute] on ChildClass.", weavingException.Message);
    }
}
