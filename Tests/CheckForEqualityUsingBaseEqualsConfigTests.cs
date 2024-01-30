using System.Xml.Linq;

public class CheckForEqualityUsingBaseEqualsConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.False(weaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.False(weaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='true'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(weaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(weaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(weaver.CheckForEqualityUsingBaseEquals);
    }
}