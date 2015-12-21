using System.Linq;

public partial class ModuleWeaver
{
    public bool InjectImplicitOnPropertyNameChanged = true;

    public void ResolveImplicitOnPropertyNameChangedConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("InjectImplicitOnPropertyNameChanged").FirstOrDefault();
            if (value != null)
            {
                InjectImplicitOnPropertyNameChanged = bool.Parse((string) value);
            }
        }
    }
}