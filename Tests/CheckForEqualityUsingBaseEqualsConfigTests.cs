using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class CheckForEqualityUsingBaseEqualsConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.IsFalse(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.IsTrue(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.IsTrue(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }
}