using System.Xml.Linq;
using Xunit;

public class TriggerDependentPropertiesConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveTriggerDependentPropertiesConfig();
        Assert.False(moduleWeaver.TriggerDependentProperties);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveTriggerDependentPropertiesConfig();
        Assert.True(moduleWeaver.TriggerDependentProperties);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(moduleWeaver.TriggerDependentProperties);
    }
}