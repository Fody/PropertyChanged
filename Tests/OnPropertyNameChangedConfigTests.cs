using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

public class OnPropertyNameChangedConfigTests :
    XunitLoggingBase
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

    public OnPropertyNameChangedConfigTests(ITestOutputHelper output) :
        base(output)
    {
    }
}