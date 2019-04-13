using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

public class CheckForEqualityConfigTests :
    XunitLoggingBase
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.False(moduleWeaver.CheckForEquality);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='0'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.False(moduleWeaver.CheckForEquality);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.True(moduleWeaver.CheckForEquality);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='1'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.True(moduleWeaver.CheckForEquality);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveCheckForEqualityConfig();
        Assert.True(moduleWeaver.CheckForEquality);
    }

    public CheckForEqualityConfigTests(ITestOutputHelper output) :
        base(output)
    {
    }
}