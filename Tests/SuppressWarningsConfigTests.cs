using System.Xml.Linq;

public class SuppressWarningsConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        Assert.False(weaver.SuppressWarnings);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        Assert.False(weaver.SuppressWarnings);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        Assert.True(weaver.SuppressWarnings);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveSuppressWarningsConfig();
        Assert.True(weaver.SuppressWarnings);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveSuppressWarningsConfig();
        Assert.False(weaver.SuppressWarnings);
    }
}