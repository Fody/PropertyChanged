using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

public class CheckForEqualityUsingBaseEqualsConfigTests :
    XunitLoggingBase
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.False(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='0'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.False(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEqualityUsingBaseEquals='1'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveCheckForEqualityUsingBaseEqualsConfig();
        Assert.True(moduleWeaver.CheckForEqualityUsingBaseEquals);
    }

    public CheckForEqualityUsingBaseEqualsConfigTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}