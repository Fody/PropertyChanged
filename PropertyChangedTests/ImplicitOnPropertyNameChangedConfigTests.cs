using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class ImplicitOnPropertyNameChangedConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectImplicitOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveImplicitOnPropertyNameChangedConfig();
        Assert.IsFalse(moduleWeaver.InjectImplicitOnPropertyNameChanged);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectImplicitOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveImplicitOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectImplicitOnPropertyNameChanged);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveImplicitOnPropertyNameChangedConfig();
        Assert.IsTrue(moduleWeaver.InjectImplicitOnPropertyNameChanged);
    }

}