using System.Xml.Linq;

public class CheckForEqualityConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        Assert.False(weaver.CheckForEquality);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        Assert.False(weaver.CheckForEquality);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        Assert.True(weaver.CheckForEquality);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityConfig();
        Assert.True(weaver.CheckForEquality);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveCheckForEqualityConfig();
        Assert.True(weaver.CheckForEquality);
    }
}