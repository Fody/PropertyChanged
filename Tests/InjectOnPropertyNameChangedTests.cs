using System.Linq;
using System.Threading.Tasks;
using Fody;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class InjectOnPropertyNameChangedTests
{
    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForNonVoidMethods()
    {
        var moduleWeaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll", assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_Warn");
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForStaticMethods()
    {
        var moduleWeaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll", assemblyName: "AssemblyWithStaticOnPropertyNameChanged_Warn");
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotWarn()
    {
        var moduleWeaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = false
        };

        var results = new[]
        {
            moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll", assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_NoWarn"),
            moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll", assemblyName: "AssemblyWithStaticOnPropertyNameChanged_NoWarn")
        };

        foreach (var result in results)
        {
            Assert.Empty(result.Warnings);
        }
    }
}