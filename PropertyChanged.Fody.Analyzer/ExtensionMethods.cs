global using static ExtensionMethods;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[assembly: InternalsVisibleTo("PropertyChanged.Fody.Ananlyzer.Tests")]

static class ExtensionMethods
{
    public static readonly SymbolDisplayFormat FullNameDisplayFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static IncrementalValuesProvider<TSource> ExceptNullItems<TSource>(this IncrementalValuesProvider<TSource?> source)
    {
        // ! We guarantee all items are not null:
        return source.Where(static item => item is not null)!;
    }

    public static IEnumerable<BaseTypeSyntax> GetInterfaceTypeCandidates(this BaseListSyntax? baseListSyntax, string name = "INotifyPropertyChanged")
    {
        return baseListSyntax == null ? Enumerable.Empty<BaseTypeSyntax>() : baseListSyntax.Types.Where(type => type.ToString().Equals(name));
    }

    public static string? NullIfEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static IEnumerable<string> EnumerateContainingTypeNames(this INamedTypeSymbol? type)
    {
        while ((type = type?.ContainingType) != null)
        {
            yield return type.Name;
        }
    }

    public static bool AreAllBaseTypesPartialClasses(this ClassDeclarationSyntax classDeclaration)
    {
        while (classDeclaration?.Parent is { } parent)
        {
            if (parent is NamespaceDeclarationSyntax or CompilationUnitSyntax)
                return true;

            if (parent is not ClassDeclarationSyntax parentClass || !parentClass.Modifiers.Any(SyntaxKind.PartialKeyword))
                return false;

            classDeclaration = parentClass;
        }

        return false;
    }

    [Conditional("DEBUG")]
    public static void Log(string message)
    {
        // File.AppendAllText(@"c:\temp\generator.log", $"{DateTime.Now}: {message}\r\n");
    }
}
