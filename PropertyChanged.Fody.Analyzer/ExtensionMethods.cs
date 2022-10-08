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

    public static IEnumerable<string> EnumerateContainingTypeDeclarations(this INamedTypeSymbol? type)
    {
        while ((type = type?.ContainingType) != null)
        {
            yield return type.GetTypeDeclaration();
        }
    }

    public static string GetTypeDeclaration(this INamedTypeSymbol type)
    {
        return $"{type.GetTypeKeyword()} {type.ToDisplayString(NameDisplayFormat)}";
    }


    static string GetTypeKeyword(this INamedTypeSymbol type)
    {
        return type.TypeKind switch
        {
            TypeKind.Interface => "interface",
            TypeKind.Class => type.IsRecord ? "record" : "class",
            TypeKind.Struct => type.IsRecord ? "record struct" : "struct",
            _ => throw new InvalidOperationException($"Unsupported type: {type.ToDisplayString(FullNameDisplayFormat)}")
        };
    }

    public static bool HasImplementationAttribute(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString().Contains("AddINotifyPropertyChangedInterface"));
    }

    public static bool HasImplementationAttribute(this INamedTypeSymbol type)
    {
        return type.GetAttributes().Any(attr => attr.AttributeClass?.ToDisplayString(FullNameDisplayFormat) == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute");
    }

    public static bool ImplementsINotifyPropertyChanged(this INamedTypeSymbol type)
    {
        return type.Interfaces.Any(item => item.ToDisplayString(FullNameDisplayFormat) == "System.ComponentModel.INotifyPropertyChanged");
    }

    public static bool HasNoPropertyChangedEvent(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.Members.OfType<EventFieldDeclarationSyntax>().SelectMany(member => member.Declaration.Variables).All(variable => variable.Identifier.Text != "PropertyChanged");
    }

    public static bool AreAllContainingTypesPartialTypes(this TypeDeclarationSyntax? typeDeclaration)
    {
        while (typeDeclaration?.Parent is { } parent)
        {
            if ((SyntaxKind)parent.RawKind is SyntaxKind.NamespaceDeclaration or SyntaxKind.FileScopedNamespaceDeclaration or SyntaxKind.CompilationUnit)
                return true;

            if (parent is not TypeDeclarationSyntax parentType || !parentType.Modifiers.Any(SyntaxKind.PartialKeyword))
                return false;

            typeDeclaration = parentType;
        }

        return false;
    }

    public static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(this INamedTypeSymbol? type)
    {
        while ((type = type?.BaseType) != null)
        {
            yield return type;
        }
    }

    [Conditional("DEBUG")]
    public static void DebugBeep()
    {
        Task.Run(Console.Beep);
    }
}
