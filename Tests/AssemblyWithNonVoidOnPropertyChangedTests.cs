public class AssemblyWithNonVoidOnPropertyChangedTests
{
    [Fact]
    public Task Simple()
    {
        var task = new ModuleWeaver();
        var result = task.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll",
            ignoreCodes: new[] {"0x80131869"});
        return Verifier.Verify(result.Warnings.Single().Text);
    }
}