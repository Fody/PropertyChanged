using Microsoft.CodeAnalysis;

class SourceGeneratorAdapter<TIncrementalGenerator> : ISourceGenerator, IIncrementalGenerator
    where TIncrementalGenerator : IIncrementalGenerator, new()
{
    readonly TIncrementalGenerator _incrementalGenerator = new();
    ISourceGenerator _sourceGenerator;

    public SourceGeneratorAdapter()
    {
        _sourceGenerator = _incrementalGenerator.AsSourceGenerator();
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _incrementalGenerator.Initialize(context);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        _sourceGenerator.Initialize(context);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        _sourceGenerator.Execute(context);
    }
}