using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Microsoft.CodeAnalysis;
using PropertyChanged;

[UsesVerify]
public partial class CodeGeneratorTest
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

        Assert.Empty(generated);
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

        Assert.Empty(generated);
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
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithFullNameInterface()
    {
        const string source = @"
public partial class Class1 : System.ComponentModel.INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialGenericClassWithoutEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial class Class1<T1, T2> : INotifyPropertyChanged
{
    public T1 Property1 { get; set; } = default!;
    public T2 Property2 { get; set; } = default!;
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialClassWithEventHandlerInDifferentPart()
    {
        const string source = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
public partial class Class1
{
    public event PropertyChangedEventHandler? PropertyChanged;
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        Assert.Empty(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialClassWithEventHandlerInDifferentPartAndDifferentSource()
    {
        const string source1 = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        const string source2 = @"
using System.ComponentModel;

public partial class Class1
{
    public event PropertyChangedEventHandler? PropertyChanged;
}
";
        var generated = await RunGenerator(source1, source2);

        Assert.Empty(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithRedundantInterfaceImplementation()
    {
        const string source1 = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
}
";
        const string source2 = @"
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source1, source2);

        await VerifyCompilation(source1, source2, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithRedundantInterfaceImplementationAndAttributeButNotOnTheFirstPart()
    {
        const string source = @"
using System.ComponentModel;
using PropertyChanged;

public partial class Class1234
{
    public string? P1 { get; set; }
}

public partial class Class1234 : INotifyPropertyChanged
{
    public string? P2 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public partial class Class1234
{
    public string? P3 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialStructWithoutEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial struct Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        Assert.Empty(generated);
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
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithAttributeInFileScopedNamespace()
    {
        const string source = @"
using PropertyChanged;

namespace Whatever;

[AddINotifyPropertyChangedInterface]
public partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialGenericClassWithAttribute()
    {
        const string source = @"
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial class Class1<T>
{
    public T Property1 { get; set; } = default!;
    public int Property2 { get; set; }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedNoneVirtualForSealedPartialClassWithAttribute()
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
        await Verify(JoinResults(generated));
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
    namespace Namespace3 
    {
        public partial class Class2a : INotifyPropertyChanged
        {
            public int Property1 { get; set; }
            public int Property2 { get; set; }
        }
    }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task CodeIsGeneratedForNestedPartialClasses()
    {
        const string source = @"
using System.ComponentModel;
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }

    public partial class Class2
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }

        public partial class Class3 : INotifyPropertyChanged
        {
            public int Property1 { get; set; }
            public int Property2 { get; set; }
        }
    }
}
";
        var generated = await RunGenerator(source);

        await VerifyCompilation(source, generated);
        await Verify(JoinResults(generated));
    }

    [Fact]
    public async Task NoCodeIsGeneratedForClassesNestedInStruct()
    {
        const string source = @"
using System.ComponentModel;

public partial struct Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }

    public partial class Class2 : INotifyPropertyChanged
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }

}
";
        var generated = await RunGenerator(source);

        Assert.Empty(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForClassesNestedInRecord()
    {
        const string source = @"
using System.ComponentModel;

public partial record Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }

    public partial class Class2 : INotifyPropertyChanged
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }
    }

}
";
        var generated = await RunGenerator(source);

        Assert.Empty(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForNestedPartialClassIfNotAllContainingClassesArePartial()
    {
        const string source = @"
using System.ComponentModel;

public partial class Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }

    public class Class2
    {
        public int Property1 { get; set; }
        public int Property2 { get; set; }

        public partial class Class3 : INotifyPropertyChanged
        {
            public int Property1 { get; set; }
            public int Property2 { get; set; }
        }
    }
}
";
        var generated = await RunGenerator(source);

        Assert.Empty(generated);
    }
}

// Test utils
partial class CodeGeneratorTest
{
    static async Task VerifyCompilation(string source, IEnumerable<GeneratedSourceResult> generated)
    {
        var sources = new[] { source }.Concat(generated.Select(item => item.SourceText.ToString()));

        await SourceGenerators.Tests.RoslynTestUtils.VerifyCompilation(references, sources);
    }
    static async Task VerifyCompilation(string source1, string source2, IEnumerable<GeneratedSourceResult> generated)
    {
        var sources = new[] { source1, source2 }.Concat(generated.Select(item => item.SourceText.ToString()));

        await SourceGenerators.Tests.RoslynTestUtils.VerifyCompilation(references, sources);
    }

    static async Task<ImmutableArray<GeneratedSourceResult>> RunGenerator(params string[] sources)
    {
        var (diagnostics, results) = await SourceGenerators.Tests.RoslynTestUtils.RunGenerator(
            new SourceGenerator(),
            references,
            sources,
            new[] { "CS0535" });

        Assert.Empty(diagnostics);

        return results;
    }

    static string JoinResults(ImmutableArray<GeneratedSourceResult> results)
    {
        return string.Join("\r\n", results.Select(result => $"// {result.HintName}\r\n{result.SourceText}"));
    }
}