using System.Linq;

public partial class ModuleWeaver
{
    public bool InjectOnPropertyNameChanged =true;


    public void ResolveOnPropertyNameChangedConfig()
    {
        var value = Config?.Attributes("InjectOnPropertyNameChanged").FirstOrDefault();
        if (value != null)
        {
            InjectOnPropertyNameChanged = bool.Parse((string) value);
        }
    }
}