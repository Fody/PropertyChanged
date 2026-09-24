using System.Xml.Linq;

public class SuppressWarningsConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        await Assert.That(weaver.SuppressWarnings).IsFalse();
    }

    [Test]
    public async Task False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        await Assert.That(weaver.SuppressWarnings).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        await Assert.That(weaver.SuppressWarnings).IsTrue();
    }

    [Test]
    public async Task True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        await Assert.That(weaver.SuppressWarnings).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveSuppressWarningsConfig();
        await Assert.That(weaver.SuppressWarnings).IsFalse();
    }
}