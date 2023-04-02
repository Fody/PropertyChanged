using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using PropertyChanged;

using static CSharpAnalyzerTestExtensions;

public class SourceGeneratorTest<TSourceGenerator> : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
    where TSourceGenerator : ISourceGenerator, new()
{
    string? _generatedSources;

    public SourceGeneratorTest(params string[] sources)
    {
        TestState.Sources.AddRange(sources.Select((source, index) => ($"File#{index}.cs", SourceText.From(source, Encoding.UTF8))));
        ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard21;
        TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck | TestBehaviors.SkipSuppressionCheck | TestBehaviors.SkipGeneratedSourcesCheck;
        SolutionTransforms.Add(AddAssemblyReferences(typeof(AddINotifyPropertyChangedInterfaceAttribute).Assembly));
    }

    public new async Task<string> RunAsync(CancellationToken cancellationToken = default)
    {
        await base.RunAsync(cancellationToken);

        return _generatedSources ?? string.Empty;
    }

    protected override async Task<Compilation> GetProjectCompilationAsync(Project project, IVerifier verifier, CancellationToken cancellationToken)
    {
        var compilation = await base.GetProjectCompilationAsync(project, verifier, cancellationToken);

        _generatedSources = JoinResults(compilation.SyntaxTrees.Skip(project.DocumentIds.Count));

        return compilation;
    }

    protected override CompilationOptions CreateCompilationOptions() => new CSharpCompilationOptions(
        OutputKind.DynamicallyLinkedLibrary,
        allowUnsafe: true,
        nullableContextOptions: NullableContextOptions.Enable);

    static string JoinResults(IEnumerable<SyntaxTree> results)
    {
        return string.Join("\r\n", results.Select(result => $"// {Path.GetFileName(result.FilePath)}\r\n{result.GetText()}"));
    }
}