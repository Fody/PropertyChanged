using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool UseStaticEqualsFromBase;

    public void ResolveUseStaticEqualsFromBaseConfig()
    {
        var value = Config?.Attributes("UseStaticEqualsFromBase")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            UseStaticEqualsFromBase = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
