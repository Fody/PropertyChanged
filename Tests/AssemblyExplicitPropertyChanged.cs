using NUnit.Framework;

[TestFixture]
public class AssemblyExplicitPropertyChanged
{
    [Test]
    public void Run()
    {
        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            new WeaverHelper("AssemblyExplicitPropertyChanged");
        });
        Assert.AreEqual("Could not inject EventInvoker method on type 'ClassExplicitPropertyChanged'. It is possible you are inheriting from a base class and have not correctly set 'EventInvokerNames' or you are using a explicit PropertyChanged event and the event field is not visible to this instance. Please either correct 'EventInvokerNames' or implement your own EventInvoker on this class. If you want to suppress this place a [DoNotNotifyAttribute] on ClassExplicitPropertyChanged.", weavingException.Message);
    }
}
