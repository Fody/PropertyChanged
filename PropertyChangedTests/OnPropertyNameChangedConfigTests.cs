using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class OnPropertyNameChangedConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.IsFalse(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectOnPropertyNameChanged);
    }
}