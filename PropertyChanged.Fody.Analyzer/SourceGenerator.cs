using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesProvider = context.SyntaxProvider.CreateSyntaxProvider(IsCandidateForGenerator, GetClassContextForCandidate).ExceptNullItems();
        var configProvider = context.AnalyzerConfigOptionsProvider.Select(ReadConfigurationSource).Select(ReadConfiguration);

        var source = classesProvider.Collect()
            .Combine(configProvider)
            .Select(((ImmutableArray<ClassContext>, Configuration) args, CancellationToken _) =>
            {
                var (classes, configuration) = args;

                return (configuration, classes);
            });

        context.RegisterSourceOutput(source, GenerateSource);
    }

    static string? ReadConfigurationSource(AnalyzerConfigOptionsProvider options, CancellationToken cancellationToken)
    {
        return options.GlobalOptions.TryGetValue("build_property.PropertyChangedAnalyzerConfiguration", out var configurationSource) ? configurationSource : null;
    }

    static Configuration ReadConfiguration(string? configuration, CancellationToken cancellationToken)
    {
        return Configuration.Read(configuration);
    }

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ImmutableArray<ClassContext> classes) parameters)
    {
        var (configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classes);
    }

    static bool IsCandidateForGenerator(SyntaxNode syntaxNode, CancellationToken token)
    {
        if (!syntaxNode.IsKind(SyntaxKind.ClassDeclaration))
            return false;

        var classDeclaration = (ClassDeclarationSyntax)syntaxNode;

        return classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
            && classDeclaration.AreAllContainingTypesPartialClasses()
            && (classDeclaration.BaseList.GetInterfaceTypeCandidates().Any() || classDeclaration.HasImplementationAttribute())
            && classDeclaration.HasNoPropertyChangedEvent();
    }

    static ClassContext? GetClassContextForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var typeSymbol = GetTypeSymbolForCandidate(context, cancellationToken);
        if (typeSymbol == null)
            return null;

        return typeSymbol.MemberNames.Contains("PropertyChanged") ? null : new ClassContext(typeSymbol);
    }

    static INamedTypeSymbol? GetTypeSymbolForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var baseTypeSyntax in classDeclarationSyntax.BaseList.GetInterfaceTypeCandidates())
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(baseTypeSyntax.Type, cancellationToken).Type;
            var fullName = typeSymbol?.ToDisplayString();

            if (fullName == "System.ComponentModel.INotifyPropertyChanged")
            {
                return context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);
            }
        }

        foreach (var attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (!attributeSyntax.Name.ToString().Contains("AddINotifyPropertyChangedInterface"))
                continue;

            if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var containingType = attributeSymbol.ContainingType;
            var fullName = containingType.ToDisplayString();

            if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
            {
                var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);
                return typeSymbol?.BaseType?.ToDisplayString(FullNameDisplayFormat) != "System.Object" ? null : typeSymbol;
            }
        }

        return null;
    }
}