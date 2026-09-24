// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class InjectOnPropertyNameChangedTests
{
    [Test]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForNonVoidMethods()
    {
        var weaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = weaver.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll",
            assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_Warn",
            ignoreCodes: ["0x80131869"]);
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Test]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForStaticMethods()
    {
        var weaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = weaver.ExecuteTestRun(
            "AssemblyWithStaticOnPropertyNameChanged.dll",
            assemblyName: "AssemblyWithStaticOnPropertyNameChanged_Warn",
            ignoreCodes: ["0x80131869"]);
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Test]
    public async Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotWarn()
    {
        var weaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = false
        };

        var results = new[]
        {
            weaver.ExecuteTestRun(
                "AssemblyWithNonVoidOnPropertyNameChanged.dll",
                assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_NoWarn",
                ignoreCodes: ["0x80131869"]),
            weaver.ExecuteTestRun(
                "AssemblyWithStaticOnPropertyNameChanged.dll",
                assemblyName: "AssemblyWithStaticOnPropertyNameChanged_NoWarn",
                ignoreCodes: ["0x80131869"])
        };

        foreach (var result in results)
        {
            await Assert.That(result.Warnings).IsEmpty();
        }
    }
}