using System.Xml.Linq;

public class CheckForSetterNonInterferenceConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged EnsureNonInterferenceWithCustomSetterBehaviors='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForSetterNonInterferenceConfig();
        Assert.False(weaver.EnsureNonInterferenceWithCustomSetterBehaviors);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged EnsureNonInterferenceWithCustomSetterBehaviors='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForSetterNonInterferenceConfig();
        Assert.False(weaver.EnsureNonInterferenceWithCustomSetterBehaviors);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged EnsureNonInterferenceWithCustomSetterBehaviors='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForSetterNonInterferenceConfig();
        Assert.True(weaver.EnsureNonInterferenceWithCustomSetterBehaviors);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged EnsureNonInterferenceWithCustomSetterBehaviors='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveCheckForSetterNonInterferenceConfig();
        Assert.True(weaver.EnsureNonInterferenceWithCustomSetterBehaviors);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveCheckForSetterNonInterferenceConfig();
        Assert.False(weaver.EnsureNonInterferenceWithCustomSetterBehaviors);
    }
}