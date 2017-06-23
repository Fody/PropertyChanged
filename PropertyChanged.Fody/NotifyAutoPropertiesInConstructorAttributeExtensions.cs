using System.Linq;
using Mono.Cecil;

static class NotifyAutoPropertiesInConstructorAttributeExtensions
{
    public static bool? ShouldNotifyAutoPropertiesInConstructor(this ICustomAttributeProvider node)
    {
        return node?
            .CustomAttributes
            .GetAttribute("PropertyChanged.NotifyAutoPropertiesInConstructorAttribute")?
            .ConstructorArguments?
            .Select(arg => arg.Value as bool?)
            .FirstOrDefault();
    }
}
