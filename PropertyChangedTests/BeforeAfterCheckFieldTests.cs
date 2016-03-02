using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class BeforeAfterCheckFieldTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged BeforeAfterCheckField='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveBeforeAfterCheckFieldConfig();
        Assert.IsFalse(moduleWeaver.BeforeAfterCheckField);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged BeforeAfterCheckField='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveBeforeAfterCheckFieldConfig();
        Assert.IsTrue(moduleWeaver.BeforeAfterCheckField);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveBeforeAfterCheckFieldConfig();
        Assert.IsFalse(moduleWeaver.BeforeAfterCheckField);
    }
}