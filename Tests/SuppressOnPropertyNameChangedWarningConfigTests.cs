using System.Xml.Linq;

public class SuppressOnPropertyNameChangedWarningConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(weaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(weaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.True(weaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressOnPropertyNameChangedWarning='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.True(weaver.SuppressOnPropertyNameChangedWarning);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveSuppressOnPropertyNameChangedWarningConfig();
        Assert.False(weaver.SuppressOnPropertyNameChangedWarning);
    }
}