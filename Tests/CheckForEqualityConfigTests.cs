using System.Xml.Linq;
using Xunit;

public class CheckForEqualityConfigTests
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
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged CheckForEquality='true'/>");
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
}