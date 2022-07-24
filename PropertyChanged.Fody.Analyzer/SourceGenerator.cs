using System.Collections.Immutable;
using System.ComponentModel;
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
        var classesProvider = context.SyntaxProvider.CreateSyntaxProvider(IsCandidateForGenerator, GetSyntaxForCandidate).ExceptNullItems();

        var input = context.CompilationProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Combine(classesProvider.Collect())
            .Select((((Compilation, AnalyzerConfigOptionsProvider), ImmutableArray<ClassDeclarationSyntax>) args, CancellationToken _) =>
            {
                var ((compilation, options), classes) = args;

                options.GlobalOptions.TryGetValue("build_property.PropertyChanged_GeneratorConfiguration", out var configurationSource);

                var configuration = Configuration.Read(configurationSource);

                return (compilation, configuration, classes);
            });

        context.RegisterSourceOutput(input, GenerateSource);
    }

    static void GenerateSource(SourceProductionContext context, (Compilation compilation, Configuration configuration, ImmutableArray<ClassDeclarationSyntax> classes) parameters)
    {
        var (compilation, configuration, classes) = parameters;

        SourceGeneratorEngine.GenerateSource(context, compilation, configuration, classes);
    }

    static bool IsCandidateForGenerator(SyntaxNode syntaxNode, CancellationToken token)
    {
        return syntaxNode is ClassDeclarationSyntax { Parent: not ClassDeclarationSyntax } classDeclaration 
               && classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) 
               && classDeclaration.Members.OfType<EventFieldDeclarationSyntax>().SelectMany(member => member.Declaration.Variables).All(variable => variable.Identifier.Text != "PropertyChanged") 
               && (classDeclaration.BaseList.GetInterfaceTypeCandidates("INotifyPropertyChanged").Any()
                   || classDeclaration.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString().Contains("AddINotifyPropertyChangedInterface")));
    }

    static ClassDeclarationSyntax? GetSyntaxForCandidate(GeneratorSyntaxContext context, CancellationToken token)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeSyntax in classDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (!attributeSyntax.Name.ToString().Contains("AddINotifyPropertyChangedInterface"))
                continue;

            if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            var fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == "PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute")
                return classDeclarationSyntax;
        }

        foreach (var baseTypeSyntax in classDeclarationSyntax.BaseList.GetInterfaceTypeCandidates("INotifyPropertyChanged"))
        {
            if (context.SemanticModel.GetSymbolInfo(baseTypeSyntax).Symbol is not ITypeSymbol typeSymbol)
                continue;

            var containingTypeSymbol = typeSymbol.ContainingType;
            var fullName = containingTypeSymbol.ToDisplayString();

            if (fullName == "System.ComponentModel.INotifyPropertyChanged")
                return classDeclarationSyntax;
        }

        return null;
    }
}