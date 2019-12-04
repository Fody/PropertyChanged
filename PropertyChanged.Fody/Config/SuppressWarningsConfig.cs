using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool SuppressWarnings;

    public void ResolveSuppressWarningsConfig()
    {
        var value = Config?.Attributes("SuppressWarnings")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            SuppressWarnings = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
