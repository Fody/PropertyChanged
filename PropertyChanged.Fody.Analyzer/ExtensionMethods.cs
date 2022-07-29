using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

static class ExtensionMethods
{
    public static IncrementalValuesProvider<TSource> ExceptNullItems<TSource>(this IncrementalValuesProvider<TSource?> source)
    {
        return source.Where(item => item is not null)!;
    }

    public static IEnumerable<BaseTypeSyntax> GetInterfaceTypeCandidates(this BaseListSyntax? baseListSyntax, string name = "INotifyPropertyChanged")
    {
        return baseListSyntax == null ? Enumerable.Empty<BaseTypeSyntax>() : baseListSyntax.Types.Where(type => type.ToString().Equals(name));
    }

    public static string? NullIfEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
