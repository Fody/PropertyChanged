using System.Xml.Linq;

public class TriggerDependentPropertiesConfigTests
{
    [Test]
    public async Task False()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='false'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveTriggerDependentPropertiesConfig();
        await Assert.That(weaver.TriggerDependentProperties).IsFalse();
    }

    [Test]
    public async Task True()
    {
        var xElement = XElement.Parse("<PropertyChanged TriggerDependentProperties='true'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveTriggerDependentPropertiesConfig();
        await Assert.That(weaver.TriggerDependentProperties).IsTrue();
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveOnPropertyNameChangedConfig();
        await Assert.That(weaver.TriggerDependentProperties).IsTrue();
    }
}