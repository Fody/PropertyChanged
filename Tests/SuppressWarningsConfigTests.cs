using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

public class SuppressWarningsConfigTests :
    XunitApprovalBase
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressWarningsConfig();
        Assert.False(moduleWeaver.SuppressWarnings);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='0'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressWarningsConfig();
        Assert.False(moduleWeaver.SuppressWarnings);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressWarningsConfig();
        Assert.True(moduleWeaver.SuppressWarnings);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged SuppressWarnings='1'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveSuppressWarningsConfig();
        Assert.True(moduleWeaver.SuppressWarnings);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveSuppressWarningsConfig();
        Assert.False(moduleWeaver.SuppressWarnings);
    }

    public SuppressWarningsConfigTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
