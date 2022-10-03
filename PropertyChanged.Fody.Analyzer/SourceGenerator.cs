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
            .Select(((ImmutableArray<TypeContext>, Configuration) args, CancellationToken _) =>
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

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ImmutableArray<TypeContext> classes) parameters)
    {
        var (configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classes);
    }

    static bool IsCandidateForGenerator(SyntaxNode syntaxNode, CancellationToken token)
    {
        if ((SyntaxKind)syntaxNode.RawKind is not (SyntaxKind.ClassDeclaration or SyntaxKind.RecordDeclaration))
            return false;

        var classDeclaration = (TypeDeclarationSyntax)syntaxNode;

        return classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
            && classDeclaration.AreAllContainingTypesPartialTypes()
            && (classDeclaration.BaseList.GetInterfaceTypeCandidates().Any() || classDeclaration.HasImplementationAttribute())
            && classDeclaration.HasNoPropertyChangedEvent();
    }

    static TypeContext? GetClassContextForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var syntax = GetSyntaxForCandidate(context, cancellationToken);
        if (syntax == null)
            return null;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken);
        if (typeSymbol == null)
            return null;

        if (typeSymbol.EnumerateBaseTypes().Any(baseType => baseType.HasImplementationAttribute() || baseType.ImplementsINotifyPropertyChanged()))
            return null;

        return typeSymbol.MemberNames.Contains("PropertyChanged") ? null : new TypeContext(typeSymbol);
    }

    static TypeDeclarationSyntax? GetSyntaxForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;

        foreach (var attributeSyntax in typeDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (!attributeSyntax.Name.ToString().Contains("AddINotifyPropertyChangedInterface"))
                continue;

            if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var containingType = attributeSymbol.ContainingType;
            var fullName = containingType.ToDisplayString(FullNameDisplayFormat);

            if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
                return typeDeclarationSyntax;
        }

        foreach (var baseTypeSyntax in typeDeclarationSyntax.BaseList.GetInterfaceTypeCandidates())
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(baseTypeSyntax.Type, cancellationToken).Type;
            var fullName = typeSymbol?.ToDisplayString(FullNameDisplayFormat);

            if (fullName == "System.ComponentModel.INotifyPropertyChanged")
                return typeDeclarationSyntax;
        }

        return null;
    }
}