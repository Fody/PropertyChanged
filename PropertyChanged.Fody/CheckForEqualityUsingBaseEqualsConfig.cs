using System.Linq;

public partial class ModuleWeaver
{
    public bool CheckForEqualityUsingBaseEquals = true;

    public void ResolveCheckForEqualityUsingBaseEqualsConfig()
    {
        var value = Config?.Attributes("CheckForEqualityUsingBaseEquals").FirstOrDefault();
        if (value != null)
        {
            CheckForEqualityUsingBaseEquals = bool.Parse((string)value);
        }
    }
}
