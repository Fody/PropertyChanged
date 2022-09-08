global using static ExtensionMethods;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[assembly: InternalsVisibleTo("PropertyChanged.Fody.Ananlyzer.Tests")]

static class ExtensionMethods
{
    public static readonly SymbolDisplayFormat FullNameDisplayFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters, miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
    public static readonly SymbolDisplayFormat NameDisplayFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameOnly, SymbolDisplayGenericsOptions.IncludeTypeParameters, miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static IncrementalValuesProvider<TSource> ExceptNullItems<TSource>(this IncrementalValuesProvider<TSource?> source)
    {
        // ! We guarantee all items are not null:
        return source.Where(static item => item is not null)!;
    }

    public static IEnumerable<BaseTypeSyntax> GetInterfaceTypeCandidates(this BaseListSyntax? baseListSyntax)
    {
        return baseListSyntax == null ? Enumerable.Empty<BaseTypeSyntax>() : baseListSyntax.Types.Where(type => type.ToString().EndsWith("INotifyPropertyChanged"));
    }

    public static string? NullIfEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static IEnumerable<string> EnumerateContainingTypeNames(this INamedTypeSymbol? type)
    {
        while ((type = type?.ContainingType) != null)
        {
            yield return type.ToDisplayString(NameDisplayFormat);
        }
    }

    public static bool HasImplementationAttribute(this ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString().Contains("AddINotifyPropertyChangedInterface"));
    }

    public static bool HasNoPropertyChangedEvent(this ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Members.OfType<EventFieldDeclarationSyntax>().SelectMany(member => member.Declaration.Variables).All(variable => variable.Identifier.Text != "PropertyChanged");
    }

    public static bool AreAllContainingTypesPartialClasses(this ClassDeclarationSyntax? classDeclaration)
    {
        while (classDeclaration?.Parent is { } parent)
        {
            if ((SyntaxKind)parent.RawKind is SyntaxKind.NamespaceDeclaration or SyntaxKind.FileScopedNamespaceDeclaration or SyntaxKind.CompilationUnit)
                return true;

            if (parent is not ClassDeclarationSyntax parentClass || !parentClass.Modifiers.Any(SyntaxKind.PartialKeyword))
                return false;

            classDeclaration = parentClass;
        }

        return false;
    }

    [Conditional("DEBUG")]
    public static void DebugBeep()
    {
        Task.Run(Console.Beep);
    }
}
