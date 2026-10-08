using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    // default: false to preserve behavior from PropertyChanged.Fody 2.x
    public bool EnsureNonInterferenceWithCustomSetterBehaviors = false;

    public void ResolveCheckForSetterNonInterferenceConfig()
    {
        var value = Config?.Attributes("EnsureNonInterferenceWithCustomSetterBehaviors")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            EnsureNonInterferenceWithCustomSetterBehaviors = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
