using System.Linq;
using System.Threading.Tasks;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class AssemblyWithNonVoidOnPropertyChangedTests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        var weavingTask = new ModuleWeaver();
        var result = weavingTask.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        return Verify(result.Warnings.Single().Text);
    }

    public AssemblyWithNonVoidOnPropertyChangedTests(ITestOutputHelper output) :
        base(output)
    {
    }
}