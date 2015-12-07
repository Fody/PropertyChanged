using System.Linq;

public partial class ModuleWeaver
{
    public bool InjectExplicitOnPropertyNameChanged = true;

    public void ResolveExplicitOnPropertyNameChangedConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("InjectExplicitOnPropertyNameChanged").FirstOrDefault();
            if (value != null)
            {
                InjectExplicitOnPropertyNameChanged = bool.Parse((string) value);
            }
        }
    }
}