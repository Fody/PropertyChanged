using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesWithAttributeProvider = context.SyntaxProvider.CreateSyntaxProvider(IsClassWithAttribute, GetSyntaxForClassWithAttribute).Where(IsNotNull);
        var classesWithInterfaceProvider = context.SyntaxProvider.CreateSyntaxProvider(IsClassWithInterface, GetSyntaxForClassWithInterface).Where(IsNotNull);

        var compilationAndClasses = context.CompilationProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Combine(classesWithAttributeProvider.Collect())
            .Combine(classesWithInterfaceProvider.Collect())
            .Select(((((Compilation, AnalyzerConfigOptionsProvider), ImmutableArray<ClassDeclarationSyntax>), ImmutableArray<ClassDeclarationSyntax>) args, CancellationToken _) =>
            {
                var (((compilation, options), classesWithAttributes), classesWithInterfaces) = args;

                options.GlobalOptions.TryGetValue("build_property.PropertyChanged_GeneratorConfiguration", out var configuration);

                return (compilation, configuration, classesWithAttributes, classesWithInterfaces);
            });

        context.RegisterSourceOutput(compilationAndClasses, GenerateSource);
    }

    void GenerateSource(SourceProductionContext context, (Compilation compilation, string configuration, ImmutableArray<ClassDeclarationSyntax> classesWithAttributes, ImmutableArray<ClassDeclarationSyntax> classesWithInterfaces) parameters)
    {
    }

    static bool IsClassWithAttribute(SyntaxNode syntaxNode, CancellationToken token)
    {
        return syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    static ClassDeclarationSyntax GetSyntaxForClassWithAttribute(GeneratorSyntaxContext context, CancellationToken token)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }

    static bool IsClassWithInterface(SyntaxNode syntaxNode, CancellationToken token)
    {
        return syntaxNode is ClassDeclarationSyntax { BaseList.Types.Count: > 0 };
    }

    static ClassDeclarationSyntax GetSyntaxForClassWithInterface(GeneratorSyntaxContext context, CancellationToken token)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var baseTypeSyntax in classDeclarationSyntax.BaseList!.Types.OfType<SimpleBaseTypeSyntax>())
        {
            if (context.SemanticModel.GetSymbolInfo(baseTypeSyntax).Symbol is not ITypeSymbol typeSymbol)
            {
                continue;
            }

            var containingTypeSymbol = typeSymbol.ContainingType;
            var fullName = containingTypeSymbol.ToDisplayString();

            if (fullName == "System.ComponentModel.INotifyPropertyChanged")
            {
                return classDeclarationSyntax;
            }
        }

        return null;
    }

    static bool IsNotNull<T>(T item)
    {
        return item is not null;
    }
}