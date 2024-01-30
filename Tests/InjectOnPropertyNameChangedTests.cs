public class InjectOnPropertyNameChangedTests
{
    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForNonVoidMethods()
    {
        var weaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = weaver.ExecuteTestRun(
            "AssemblyWithNonVoidOnPropertyNameChanged.dll",
            assemblyName: "AssemblyWithNonVoidOnPropertyNameChanged_Warn",
            ignoreCodes: new[] {"0x80131869"});
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public Task ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_WarnsForStaticMethods()
    {
        var weaver = new ModuleWeaver
        {
            InjectOnPropertyNameChanged = true
        };

        var result = weaver.ExecuteTestRun(
            "AssemblyWithStaticOnPropertyNameChanged.dll",
            assemblyName: "AssemblyWithStaticOnPropertyNameChanged_Warn",
            ignoreCodes: new[] {"0x80131869"});
        return Verifier.Verify(result.Warnings.Single().Text);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotWarn()
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
                ignoreCodes: new[] {"0x80131869"}),
            weaver.ExecuteTestRun(
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