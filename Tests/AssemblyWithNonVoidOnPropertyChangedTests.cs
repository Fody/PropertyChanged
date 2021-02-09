using System.Linq;
using System.Threading.Tasks;
using Fody;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class AssemblyWithNonVoidOnPropertyChangedTests
{
    [Fact]
    public Task Simple()
    {
        var weavingTask = new ModuleWeaver();
        var result = weavingTask.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll",
            ignoreCodes: new[] {"0x80131869"});
        return Verifier.Verify(result.Warnings.Single().Text);
    }
}