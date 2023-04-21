using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Test = SourceGeneratorTest<SourceGenerator>;

[UsesVerify]
public class CodeGeneratorTest
{
    static readonly DiagnosticResult CS0535 = new DiagnosticResult("CS0535", DiagnosticSeverity.Error).WithLocation(0);

    static CodeGeneratorTest()
    {
        SourceGeneratorEngine.GeneratorVersion = "TEST";
    }

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
        var generated = await new Test(source).RunAsync();

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
        var generated = await new Test(source).RunAsync();

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
        var generated = await new Test(source).RunAsync();
        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsNotGeneratedForPartialRecordWithoutEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial record Class1 : {|#0:INotifyPropertyChanged|}
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var test = new Test(source)
        {
            ExpectedDiagnostics = {CS0535.WithArguments("Class1", "System.ComponentModel.INotifyPropertyChanged.PropertyChanged")}
        };

        var generated = await test.RunAsync();

        Assert.Empty(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

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
        var generated = await new Test(source1, source2).RunAsync();

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
        var generated = await new Test(source1, source2).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialStructWithoutEventHandler()
    {
        const string source = @"
using System.ComponentModel;

public partial struct Class1 : {|#0:INotifyPropertyChanged|}
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var test = new Test(source)
        {
            ExpectedDiagnostics = {CS0535.WithArguments("Class1", "System.ComponentModel.INotifyPropertyChanged.PropertyChanged")}
        };

        var generated = await test.RunAsync();

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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsNotGeneratedForPartialRecordWithAttribute()
    {
        const string source = @"
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial record Class1
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await new Test(source).RunAsync();

        Assert.Empty(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForPartialClassWithAttributeAndInterfaceAndBaseClass()
    {
        const string source = @"
using PropertyChanged;
using System;
using System.ComponentModel;

[AddINotifyPropertyChangedInterface]
public partial class Class1 : Attribute, INotifyPropertyChanged
{
    public int Property1 { get; set; }
    public int Property2 { get; set; }
}
";
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialClass2WithAttributeAndAttributedBaseClass()
    {
        const string source = @"
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public partial class Class1
{
    public int Property1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public partial class Class2 : Class1
{
    public int Property2 { get; set; }
}
";
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task NoCodeIsGeneratedForPartialClass2WithAttributeAndInterfaceImplementationInBaseClass()
    {
        const string source = @"
using PropertyChanged;
using System.ComponentModel;

public partial class Class1 : INotifyPropertyChanged
{
    public int Property1 { get; set; }
}

[AddINotifyPropertyChangedInterface]
public partial class Class2 : Class1
{
    public int Property2 { get; set; }
}
";
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForClassesNestedInStruct()
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForClassesNestedInRecord()
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
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
    }

    [Fact]
    public async Task CodeIsGeneratedForDeepNestedItems()
    {
        const string source = @"
using System.ComponentModel;

partial record Level1
{
    partial struct Level2 
    {
        partial interface Level3
        {
            partial record struct Level4
            {
                partial class Level5
                {
                    partial class Class : INotifyPropertyChanged
                    {
                        public int Property1 { get; set; }
                        public int Property2 { get; set; }
                    }
                }
            }
        }
    }
}
";
        var generated = await new Test(source).RunAsync();

        await Verify(generated);
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

        public partial class Class3 : {|#0:INotifyPropertyChanged|}
        {
            public int Property1 { get; set; }
            public int Property2 { get; set; }
        }
    }
}
";
        var test = new Test(source)
        {
            ExpectedDiagnostics = {CS0535.WithArguments("Class1.Class2.Class3", "System.ComponentModel.INotifyPropertyChanged.PropertyChanged")}
        };

        var generated = await test.RunAsync();

        Assert.Empty(generated);
    }
}