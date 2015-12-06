using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class ResolveSearchOnPropertyNameChangedMethodsByNamingConventionConfigTests
{
    [Test]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged SearchOnPropertyNameChangedMethodsByNamingConvention='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSearchOnPropertyNameChangedMethodsByNamingConventionConfig();
        Assert.IsFalse(moduleWeaver.SearchOnPropertyNameChangedMethodsByNamingConvention);
    }

    [Test]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged SearchOnPropertyNameChangedMethodsByNamingConvention='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSearchOnPropertyNameChangedMethodsByNamingConventionConfig();
        Assert.IsTrue(moduleWeaver.SearchOnPropertyNameChangedMethodsByNamingConvention);
    }

    [Test]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveSearchOnPropertyNameChangedMethodsByNamingConventionConfig();
        Assert.IsTrue(moduleWeaver.SearchOnPropertyNameChangedMethodsByNamingConvention);
    }

}
