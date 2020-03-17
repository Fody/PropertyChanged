using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool SuppressOnPropertyNameChangedWarning;

    public void ResolveSuppressOnPropertyNameChangedWarningConfig()
    {
        var value = Config?.Attributes("SuppressOnPropertyNameChangedWarning")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            SuppressOnPropertyNameChangedWarning = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
