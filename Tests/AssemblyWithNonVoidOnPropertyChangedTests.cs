// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithNonVoidOnPropertyChangedTests
{
    [Test]
    public Task Simple()
    {
        var task = new ModuleWeaver();
        var result = task.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll",
            ignoreCodes: ["0x80131869"]);
        return Verifier.Verify(result.Warnings.Single().Text);
    }
}