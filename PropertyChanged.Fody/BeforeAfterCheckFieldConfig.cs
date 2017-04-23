using System.Linq;

public partial class ModuleWeaver
{
    public bool BeforeAfterCheckField = false;

    public void ResolveBeforeAfterCheckFieldConfig()
    {
        var value = Config?.Attributes("BeforeAfterCheckField").FirstOrDefault();
        if (value != null)
        {
            BeforeAfterCheckField = bool.Parse((string)value);
        }
    }
}