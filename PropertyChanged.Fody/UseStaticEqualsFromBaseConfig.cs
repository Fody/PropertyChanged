using System.Linq;

public partial class ModuleWeaver
{
    public bool UseStaticEqualsFromBase;

    public void ResolveUseStaticEqualsFromBaseConfig()
    {
        var value = Config?.Attributes("UseStaticEqualsFromBase").FirstOrDefault();
        if (value != null)
        {
            UseStaticEqualsFromBase = bool.Parse((string)value);
        }
    }
}
