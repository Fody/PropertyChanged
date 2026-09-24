using System.Xml.Linq;

public class OnPropertyNameChangedConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var weaver = new ModuleWeaver {Config = xElement};
        weaver.ResolveOnPropertyNameChangedConfig();
        await Assert.That(weaver.InjectOnPropertyNameChanged).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var weaver = new ModuleWeaver {Config = xElement};
        weaver.ResolveOnPropertyNameChangedConfig();
        await Assert.That(weaver.InjectOnPropertyNameChanged).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveOnPropertyNameChangedConfig();
        await Assert.That(weaver.InjectOnPropertyNameChanged).IsTrue();
    }
}