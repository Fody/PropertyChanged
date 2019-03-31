using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool InjectOnPropertyNameChanged = true;

    public void ResolveOnPropertyNameChangedConfig()
    {
        var value = Config?.Attributes("InjectOnPropertyNameChanged")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            InjectOnPropertyNameChanged = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}