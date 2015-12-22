using System.Linq;

public partial class ModuleWeaver
{
    public bool CheckForEquality = true;

    public void ResolveCheckForEqualityConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("CheckForEquality").FirstOrDefault();
            if (value != null)
            {
                CheckForEquality = bool.Parse((string)value);
            }
        }
    }
}
