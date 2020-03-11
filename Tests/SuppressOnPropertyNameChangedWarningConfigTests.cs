using System.Xml.Linq;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SuppressOnPropertyNameChangedWarningConfigTests :
    VerifyBase
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(moduleWeaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='0'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(moduleWeaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.True(moduleWeaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='1'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.True(moduleWeaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(moduleWeaver.SuppressOnPropertyNameChangedWarning);
    }

    public SuppressOnPropertyNameChangedWarningConfigTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
