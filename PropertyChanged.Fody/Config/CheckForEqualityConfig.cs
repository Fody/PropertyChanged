using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool CheckForEquality = true;

    public void ResolveCheckForEqualityConfig()
    {
        var value = Config?.Attributes("CheckForEquality").Select(a => a.Value).FirstOrDefault();
        if (value != null)
        {
            CheckForEquality = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
