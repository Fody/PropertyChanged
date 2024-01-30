using System.Xml.Linq;

public class TriggerDependentPropertiesConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='false'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveTriggerDependentPropertiesConfig();
        Assert.False(weaver.TriggerDependentProperties);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='true'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveTriggerDependentPropertiesConfig();
        Assert.True(weaver.TriggerDependentProperties);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(weaver.TriggerDependentProperties);
    }
}