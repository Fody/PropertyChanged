using System.Xml.Linq;

public class CheckForEqualityConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        await Assert.That(weaver.CheckForEquality).IsFalse();
    }

    [Test]
    public async Task False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        await Assert.That(weaver.CheckForEquality).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        await Assert.That(weaver.CheckForEquality).IsTrue();
    }

    [Test]
    public async Task True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        await Assert.That(weaver.CheckForEquality).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveCheckForEqualityConfig();
        await Assert.That(weaver.CheckForEquality).IsTrue();
    }
}