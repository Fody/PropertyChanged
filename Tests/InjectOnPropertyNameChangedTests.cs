using System.Linq;
using System.Threading.Tasks;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class InjectOnPropertyNameChangedTests :
    VerifyBase
{
    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_ThrowsWeavingExceptionForNonVoidMethods()
    {
        var moduleWeaver = new ModuleWeaver {InjectOnPropertyNameChanged = true};

        var result = moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        return Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_ThrowsWeavingExceptionForStaticMethods()
    {
        var moduleWeaver = new ModuleWeaver {InjectOnPropertyNameChanged = true};

        var result = moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll");
        return Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotThrowWeavingException()
    {
        var moduleWeaver = new ModuleWeaver { InjectOnPropertyNameChanged = false };

        moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll");
    }

    public InjectOnPropertyNameChangedTests(ITestOutputHelper output) :
        base(output)
    {
    }
}