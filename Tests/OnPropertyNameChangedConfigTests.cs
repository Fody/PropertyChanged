using System.Xml.Linq;
using Xunit;


public class OnPropertyNameChangedConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.False(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanged);
    }
}