using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

public static class CSharpAnalyzerTestExtensions
{
    public static Func<Solution, ProjectId, Solution> CreateSolutionTransform(Func<Solution, Project, Solution> transform)
    {
        return (solution, projectId) =>
        {
            var project = solution.GetProject(projectId) ?? throw new InvalidOperationException("Project is not part of the solution");
            return transform(solution, project);
        };
    }

    public static Func<Solution, ProjectId, Solution> AddAssemblyReferences(params Assembly[] assemblies) => (solution, projectId) =>
    {
        var metadataReferences = assemblies
            .Distinct()
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        return solution.AddMetadataReferences(projectId, metadataReferences);
    };

    public static Func<Solution, ProjectId, Solution> UseLanguageVersion(LanguageVersion languageVersion) => CreateSolutionTransform((solution, project) =>
    {
        var parseOptions = project.ParseOptions as CSharpParseOptions ?? throw new InvalidOperationException("Project does not have CSharpParseOptions");

        return solution.WithProjectParseOptions(project.Id, parseOptions.WithLanguageVersion(languageVersion));
    });

    public static Func<Solution, ProjectId, Solution> WithProjectCompilationOptions(Func<CSharpCompilationOptions, CSharpCompilationOptions> callback) => CreateSolutionTransform((solution, project) =>
    {
        var compilationOptions = project.CompilationOptions as CSharpCompilationOptions ?? throw new InvalidOperationException("Project does not have CSharpCompilationOptions");

        return solution.WithProjectCompilationOptions(project.Id, callback(compilationOptions));
    });

    public static DiagnosticResult AsResult(this DiagnosticDescriptor descriptor) => new(descriptor);

    public static CSharpCompilationOptions AsCSharpCompilationOptions(this CompilationOptions options) => (CSharpCompilationOptions)options;

    public static CSharpCompilationOptions WithCSharpDefaults(this CompilationOptions options) => options
        .AsCSharpCompilationOptions()
        .WithNullableWarningsAsErrors()
        .WithNullableContextOptions(NullableContextOptions.Enable);

    public static ReferenceAssemblies AddPackages(this ReferenceAssemblies referenceAssemblies, params PackageIdentity[] packages)
    {
        return referenceAssemblies.AddPackages(packages.ToImmutableArray());
    }

    public static CSharpCompilationOptions WithNullableWarningsAsErrors(this CSharpCompilationOptions options)
    {
        return options.WithSpecificDiagnosticOptions(options.SpecificDiagnosticOptions.SetItems(NullableWarningsAsErrors));
    }

    public static ImmutableDictionary<string, ReportDiagnostic> NullableWarningsAsErrors { get; } = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
        return nullableWarnings;
    }
}