using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool TriggerDependentProperties = true;

    public void ResolveTriggerDependentPropertiesConfig()
    {
        var value = Config.Attributes("TriggerDependentProperties")
            .Select(_ => _.Value)
            .SingleOrDefault();
        if (value != null)
        {
            TriggerDependentProperties = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}