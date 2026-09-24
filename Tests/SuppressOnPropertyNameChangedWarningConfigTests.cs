using System.Xml.Linq;

public class SuppressOnPropertyNameChangedWarningConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        await Assert.That(weaver.SuppressOnPropertyNameChangedWarning).IsFalse();
    }

    [Test]
    public async Task False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        await Assert.That(weaver.SuppressOnPropertyNameChangedWarning).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        await Assert.That(weaver.SuppressOnPropertyNameChangedWarning).IsTrue();
    }

    [Test]
    public async Task True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        await Assert.That(weaver.SuppressOnPropertyNameChangedWarning).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        await Assert.That(weaver.SuppressOnPropertyNameChangedWarning).IsFalse();
    }
}