public class SourceGeneratorTest<TSourceGenerator> :
    CSharpIncrementalGeneratorSnapshotTest<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public SourceGeneratorTest(params string[] sources)
    {
        TestState.Sources.AddRange(sources.Select((source, index) => ($"File#{index}.cs", SourceText.From(source, Encoding.UTF8))));
        ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard21;
        TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck | TestBehaviors.SkipSuppressionCheck | TestBehaviors.SkipGeneratedSourcesCheck;

        this.AddReferences(typeof(AddINotifyPropertyChangedInterfaceAttribute).Assembly);
    }
}
