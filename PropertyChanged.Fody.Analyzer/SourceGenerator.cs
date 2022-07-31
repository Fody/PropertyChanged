// #define ONE_FILE_PER_CLASS

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
        var configProvider = context.AnalyzerConfigOptionsProvider.Select(ReadConfigurationSource);

#if ONE_FILE_PER_CLASS
        
        var source = classesProvider
            .Combine(configProvider)
            .Select(((ClassContext, string?) args, CancellationToken _) =>
            {
                var (classContext, configurationSource) = args;

                var configuration = Configuration.Read(configurationSource);

                return (configuration, classContext);
            });
#else        
        var source = classesProvider.Collect()
            .Combine(configProvider)
            .Select(((ImmutableArray<ClassContext>, string?) args, CancellationToken _) =>
            {
                var (classes, configurationSource) = args;

                var configuration = Configuration.Read(configurationSource);

                return (configuration, classes);
            });
#endif

        context.RegisterSourceOutput(source, GenerateSource);
    }

    string? ReadConfigurationSource(AnalyzerConfigOptionsProvider options, CancellationToken cancellationToken)
    {
        return options.GlobalOptions.TryGetValue("build_property.PropertyChanged_GeneratorConfiguration", out var configurationSource) ? configurationSource : null;
    }

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ImmutableArray<ClassContext> classes) parameters)
    {
        var (configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classes);
    }

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ClassContext classContext) parameters)
    {
        var (configuration, classContext) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classContext);
    }

    static bool IsCandidateForGenerator(SyntaxNode syntaxNode, CancellationToken token)
    {
        try
        {
            if (syntaxNode is not ClassDeclarationSyntax classDeclaration)
                return false;

            return classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
                && classDeclaration.Members.OfType<EventFieldDeclarationSyntax>().SelectMany(member => member.Declaration.Variables).All(variable => variable.Identifier.Text != "PropertyChanged")
                && classDeclaration.AreAllBaseTypesPartialClasses()
                && (classDeclaration.BaseList.GetInterfaceTypeCandidates().Any()
                   || classDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString().Contains("AddINotifyPropertyChangedInterface")));
        }
        catch (Exception ex)
        {
            Log("IsCandidateForGenerator: " + ex);
            return false;
        }
    }

    static ClassContext? GetClassContextForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var syntax = GetSyntaxForCandidate(context, cancellationToken);
        if (syntax == null)
            return null;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken);
        if (typeSymbol == null)
            return null;

        if (typeSymbol.MemberNames.Contains("PropertyChanged"))
            return null;

#if ONE_FILE_PER_CLASS
        var syntaxReferences = typeSymbol.DeclaringSyntaxReferences;
        if (syntaxReferences.Length > 1)
        {
            // We must not generate code twice for the same class; if there are multiple partial candidates, only allow the first to pass through.
            // This is a very simple check; if the first one is not a candidate, no code will be generated at all, but that's a very weird edge case
            // and we should not spend too much efforts to support it.
            var first = syntaxReferences[0];
            if ((first.SyntaxTree != syntax.SyntaxTree) || (first.Span != syntax.Span))
                return null;
        }
#endif

        Log($"GetClassContextForCandidate: {syntax.Identifier.Text}");

        return new ClassContext(syntax, typeSymbol);
    }

    static ClassDeclarationSyntax? GetSyntaxForCandidate(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        try
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            foreach (var attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
            {
                if (!attributeSyntax.Name.ToString().Contains("AddINotifyPropertyChangedInterface"))
                    continue;

                if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var containingType = attributeSymbol.ContainingType;
                var fullName = containingType.ToDisplayString();

                if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
                    return classDeclarationSyntax;
            }

            foreach (var baseTypeSyntax in classDeclarationSyntax.BaseList.GetInterfaceTypeCandidates())
            {
                var typeSymbol = context.SemanticModel.GetTypeInfo(baseTypeSyntax.Type, cancellationToken).Type;
                var fullName = typeSymbol?.ToDisplayString();

                if (fullName == "System.ComponentModel.INotifyPropertyChanged")
                    return classDeclarationSyntax;
            }

            return null;
        }
        catch (Exception ex)
        {
            Log("GetSyntaxForCandidate: " + ex);
            return null;
        }
    }
}