using System.Linq;

public partial class ModuleWeaver
{
    public bool InjectOnPropertyNameChanged =true;


    public void ResolveOnPropertyNameChangedConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("InjectOnPropertyNameChanged").FirstOrDefault();
            if (value != null)
            {
                InjectOnPropertyNameChanged = bool.Parse((string) value);
            }
        }
    }
}