using System.ComponentModel;
using System.Reflection;
using PropertyChanged;

[UsesVerify]
public class CodeGeneratorTest
{
    static readonly Assembly[] references = { typeof(AddINotifyPropertyChangedInterfaceAttribute).Assembly, typeof(INotifyPropertyChanged).Assembly };

    [Fact]
    public async Task NoCodeIsGeneratedForNonPartialClass()
    {
        const string source = @"
using System.ComponentModel;

public class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}
";
        var generated = await RunGenerator(source);

        await Verify(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialClassWithEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}
";
        var generated = await RunGenerator(source);

        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithoutEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithAttribute()
    {
        const string source = @"
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(generated);
    }

    [Fact]
    public async Task NoneVirtualCodeIsGeneratedForSealedPartialClassWithAttribute()
    {
        const string source = @"
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public sealed partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForClassesInMultipleNamespaces()
    {
        const string source = @"
using System.ComponentModel;
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
public partial class Class2 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
namespace Namespace1 
{
    [AddINotifyPropertyChangedInterface]
    public partial class Class1
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }
    public partial class Class2 : INotifyPropertyChanged
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }
}
namespace Namespace2 
{
    [AddINotifyPropertyChangedInterface]
    public partial class Class1
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }
    public partial class Class2 : INotifyPropertyChanged
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(generated);
    }

    /// <summary>
    /// Verifies the sources compile with no errors.
    /// </summary>
    /// <param name="sources">The sources.</param>
    static async Task VerifyCompilation(params string[] sources)
    {
        await SourceGenerators.Tests.RoslynTestUtils.VerifyCompilation(references, sources);
    }

    static async Task<string> RunGenerator(params string[] sources)
    {
        var (diagnostics, results) = await SourceGenerators.Tests.RoslynTestUtils.RunGenerator(
            new SourceGenerator(),
            references,
            sources,
            new[] { "CS0535" });

        Assert.Empty(diagnostics);
        Assert.Single(results);

        var generated = results[0].SourceText.ToString();


        return generated;
    }
}
