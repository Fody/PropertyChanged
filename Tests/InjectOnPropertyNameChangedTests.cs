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

        var result = moduleWeaver.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll", 
            assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_Warn",
            ignoreCodes: new[] {"0x80131869"});
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForStaticMethods()
    {
        var moduleWeaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = moduleWeaver.ExecuteTestRun(
            "AssemblyWithStaticOnPropertyNameChanged.dll",
            assemblyName: "AssemblyWithStaticOnPropertyNameChanged_Warn",
            ignoreCodes: new[] {"0x80131869"});
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
            moduleWeaver.ExecuteTestRun(
                "AssemblyWithNonVoidOnPropertyNameChanged.dll",
                assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_NoWarn",
                ignoreCodes: new[] {"0x80131869"}),
            moduleWeaver.ExecuteTestRun(
                "AssemblyWithStaticOnPropertyNameChanged.dll",
                assemblyName: "AssemblyWithStaticOnPropertyNameChanged_NoWarn",
                ignoreCodes: new[] {"0x80131869"})
        };

        foreach (var result in results)
        {
            Assert.Empty(result.Warnings);
        }
    }
}