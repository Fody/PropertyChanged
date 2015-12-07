using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class ExplicitOnPropertyNameChangedConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectExplicitOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveExplicitOnPropertyNameChangedConfig();
        Assert.IsFalse(moduleWeaver.InjectExplicitOnPropertyNameChanged);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectExplicitOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveExplicitOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectExplicitOnPropertyNameChanged);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveExplicitOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectExplicitOnPropertyNameChanged);
    }

}