using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool CheckForEqualityUsingBaseEquals = true;

    public void ResolveCheckForEqualityUsingBaseEqualsConfig()
    {
        var value = Config?.Attributes("CheckForEqualityUsingBaseEquals")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            CheckForEqualityUsingBaseEquals = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
