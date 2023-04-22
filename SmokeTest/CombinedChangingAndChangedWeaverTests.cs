#if NETFRAMEWORK

namespace SmokeTest;

using System.Reflection.Metadata;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Metadata;
using PropertyChanged;
using PropertyChanging;
using VerifyTests.ICSharpCode.Decompiler;
using VerifyXunit;

[UsesVerify]
public sealed class CombinedChangingAndChangedWeaverTests : IDisposable
{
    PEFile _file = new(typeof(Testee).Assembly.Location);

    static CombinedChangingAndChangedWeaverTests()
    {
        VerifyICSharpCodeDecompiler.Initialize();
    }

    MethodToDisassemble GetPropertySetter(string propertyName)
    {
        var setter = _file.FindTypeDefinition("SmokeTest.Testee")
            .Properties.Where(p => p.Name == propertyName)
            .Select(pr => pr.Setter?.MetadataToken)
            .FirstOrDefault() ?? throw new InvalidOperationException("Property does not exist");

        return new MethodToDisassemble(_file, (MethodDefinitionHandle)setter);
    }

    [Fact]
    public async Task ReferenceTypeProperty()
    {
        await Verify(GetPropertySetter("Property1")).UniqueForAssemblyConfiguration();
    }

    [Fact]
    public async Task ValueTypeProperty()
    {
        await Verify(GetPropertySetter("Property2")).UniqueForAssemblyConfiguration();
    }

    [Fact]
    public async Task NullableValueTypeProperty()
    {
        await Verify(GetPropertySetter("Property3")).UniqueForAssemblyConfiguration();
    }

    public void Dispose()
    {
        _file.Dispose();
    }
}

[ImplementPropertyChanging]
[AddINotifyPropertyChangedInterface]
public class Testee
{
    public string Property1 { get; set; }

    public int Property2 { get; set; }

    public int? Property3 { get; set; }
}

#endif

