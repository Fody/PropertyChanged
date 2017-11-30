using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class CheckForEqualityConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.IsFalse(moduleWeaver.CheckForEquality);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.IsTrue(moduleWeaver.CheckForEquality);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.IsTrue(moduleWeaver.CheckForEquality);
    }
}