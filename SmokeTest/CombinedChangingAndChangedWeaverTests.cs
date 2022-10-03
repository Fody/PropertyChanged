#if NETFRAMEWORK

namespace SmokeTest;

using System.Threading.Tasks;
using Fody;
using PropertyChanged;
using PropertyChanging;
using VerifyXunit;


[UsesVerify]
public class CombinedChangingAndChangedWeaverTests
{
    [Fact]
    public async Task ReferenceTypeProperty()
    {
        var result = Ildasm.Decompile(typeof(Testee).Assembly.Location, "SmokeTest.Testee::set_Property1");

        await Verify(result).UniqueForAssemblyConfiguration();
    }

    [Fact]
    public async Task ValueTypeProperty()
    {
        var result = Ildasm.Decompile(typeof(Testee).Assembly.Location, "SmokeTest.Testee::set_Property2");

        await Verify(result).UniqueForAssemblyConfiguration();
    }

    [Fact]
    public async Task NullableValueTypeProperty()
    {
        var result = Ildasm.Decompile(typeof(Testee).Assembly.Location, "SmokeTest.Testee::set_Property3");

        await Verify(result).UniqueForAssemblyConfiguration();
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

