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

        //*
        var source = classesProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(((ClassContext, AnalyzerConfigOptionsProvider) args, CancellationToken _) =>
            {
                var (classContext, options) = args;

                options.GlobalOptions.TryGetValue("build_property.PropertyChanged_GeneratorConfiguration", out var configurationSource);

                var configuration = Configuration.Read(configurationSource);

                return (configuration, classContext);
            });
        /*/
        var source = context.AnalyzerConfigOptionsProvider
            .Combine(classesProvider.Collect())
            .Select(((AnalyzerConfigOptionsProvider, ImmutableArray<ClassContext>) args, CancellationToken _) =>
            {
                var (options, classes) = args;

                options.GlobalOptions.TryGetValue("build_property.PropertyChanged_GeneratorConfiguration", out var configurationSource);

                var configuration = Configuration.Read(configurationSource);

                return (configuration, classes);
            });
        //*/

        context.RegisterSourceOutput(source, GenerateSource);
    }

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ImmutableArray<ClassContext> classes) parameters)
    {
        var (configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classes);
    }

    static void GenerateSource(SourceProductionContext context, (Configuration configuration, ClassContext classContext) parameters)
    {
        var (configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, configuration, classes);
    }

    static bool IsCandidateForGenerator(SyntaxNode syntaxNode, CancellationToken token)
    {
        try
        {
            return syntaxNode is ClassDeclarationSyntax { Parent: NamespaceDeclarationSyntax or CompilationUnitSyntax } classDeclaration
                   && classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
                   && classDeclaration.Members.OfType<EventFieldDeclarationSyntax>().SelectMany(member => member.Declaration.Variables).All(variable => variable.Identifier.Text != "PropertyChanged")
                   && (classDeclaration.BaseList.GetInterfaceTypeCandidates().Any()
                       || classDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString().Contains("AddINotifyPropertyChangedInterface")));
        }
        catch (Exception ex)
        {
            Log("IsCandidateForGenerator: " + ex);
            return false;
        }
    }

    static ClassContext? GetClassContextForCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        var syntax = GetSyntaxForCandidate(context, token);
        if (syntax == null)
            return null;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(syntax, token);
        if (typeSymbol == null)
            return null;

        if (typeSymbol.MemberNames.Contains("PropertyChanged"))
            return null;

        var syntaxReferences = typeSymbol.DeclaringSyntaxReferences;
        if ((syntaxReferences.Length > 1) && ((syntax.SyntaxTree != syntaxReferences[0].SyntaxTree) || syntaxReferences[0].Span != syntax.Span))
        {
            // We must not generate code twice for the same class; if there are multiple partial candidates, only allow the first to pass through.
            // This is a very simple check; if the first one is not a candidate, no code will be generated at all, but that's a very weird edge case
            // and we should not spend too much efforts to support it.
            return null;
        }

        Log($"GetClassContextForCandidate: {syntax.Identifier.Text}");

        return new ClassContext(syntax, typeSymbol);
    }

    static ClassDeclarationSyntax? GetSyntaxForCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        try
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            foreach (var attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
            {
                if (!attributeSyntax.Name.ToString().Contains("AddINotifyPropertyChangedInterface"))
                    continue;

                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var containingType = attributeSymbol.ContainingType;
                var fullName = containingType.ToDisplayString();

                if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
                    return classDeclarationSyntax;
            }

            foreach (var baseTypeSyntax in classDeclarationSyntax.BaseList.GetInterfaceTypeCandidates())
            {
                var typeSymbol = context.SemanticModel.GetTypeInfo(baseTypeSyntax.Type).Type;
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