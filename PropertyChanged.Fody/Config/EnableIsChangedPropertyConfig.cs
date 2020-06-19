using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool EnableIsChangedProperty = true;

    public void ResolveEnableIsChangedPropertyConfig()
    {
        var value = Config?.Attributes("EnableIsChangedProperty")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            EnableIsChangedProperty = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}
