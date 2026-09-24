using System.Xml.Linq;

public class CheckForEqualityUsingBaseEqualsConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        await Assert.That(weaver.CheckForEqualityUsingBaseEquals).IsFalse();
    }

    [Test]
    public async Task False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        await Assert.That(weaver.CheckForEqualityUsingBaseEquals).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='true'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        await Assert.That(weaver.CheckForEqualityUsingBaseEquals).IsTrue();
    }

    [Test]
    public async Task True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        await Assert.That(weaver.CheckForEqualityUsingBaseEquals).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        await Assert.That(weaver.CheckForEqualityUsingBaseEquals).IsTrue();
    }
}