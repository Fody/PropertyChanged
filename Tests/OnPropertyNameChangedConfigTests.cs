using System.Xml.Linq;

public class OnPropertyNameChangedConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var weaver = new ModuleWeaver {Config = xElement};
        weaver.ResolveOnPropertyNameChangedConfig();
        Assert.False(weaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var weaver = new ModuleWeaver {Config = xElement};
        weaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(weaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(weaver.InjectOnPropertyNameChanged);
    }
}