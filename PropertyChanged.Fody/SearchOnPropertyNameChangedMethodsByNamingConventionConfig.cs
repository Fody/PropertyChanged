using System.Linq;

public partial class ModuleWeaver
{
    public bool SearchOnPropertyNameChangedMethodsByNamingConvention = true;

    public void ResolveSearchOnPropertyNameChangedMethodsByNamingConventionConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("SearchOnPropertyNameChangedMethodsByNamingConvention").FirstOrDefault();
            if (value != null)
            {
                SearchOnPropertyNameChangedMethodsByNamingConvention = bool.Parse((string)value);
            }
        }
    }
}