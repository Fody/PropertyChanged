public class AssemblyWithSetterSideEffectsTests
{
    static readonly string[] peVerifyIgnoreCodes =
    {
#if NETCOREAPP
        "0x80131869"
#endif
    };
    
    const string assemblyName = "AssemblyWithSetterSideEffects.dll";
    
    [Theory]
#if TEMP_REQUIRE_IMPLEMENTATION_SETTER_NON_INTERFERENCE
    [InlineData([true, "same", "same", 2])]
#endif
    [InlineData([true, "different1", "different2", 2])]
    [InlineData([false, "same", "same", 2])]
    [InlineData([false, "different1", "different2", 2])]
    public void CallsPreAssignmentSideEffect(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectBeforeValueAssignment";

        var weaver = new ModuleWeaver(){CheckForEquality = checkForEquality, EnsureNonInterferenceWithCustomSetterBehaviors = true };
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);
        
        var instance = testResult.GetInstance(className);

        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;
        
        var callCount = (int)instance.SideEffectBeforeCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }

    [Theory]
#if TEMP_REQUIRE_IMPLEMENTATION_SETTER_NON_INTERFERENCE
    [InlineData([true, "same", "same", 2])]
#endif
    [InlineData([true, "different1", "different2", 2])]
    [InlineData([false, "same", "same", 2])]
    [InlineData([false, "different1", "different2", 2])]
    public void CallsPostAssignmentSideEffect(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectAfterValueAssignment";

        var weaver = new ModuleWeaver() { CheckForEquality = checkForEquality, EnsureNonInterferenceWithCustomSetterBehaviors = true };
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);
        
        var instance = testResult.GetInstance(className);
        
        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;
        
        var callCount = (int)instance.SideEffectAfterCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }

    [Theory]
    [InlineData([true, "same", "same", 1])]
    [InlineData([true, "different1", "different2", 2])]
    [InlineData([false, "same", "same", 2])]
    [InlineData([false, "different1", "different2", 2])]
    public void CallsPreAssignmentSideEffectLegacy(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectBeforeValueAssignment";

        var weaver = new ModuleWeaver() { CheckForEquality = checkForEquality, EnsureNonInterferenceWithCustomSetterBehaviors = false };
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);

        var instance = testResult.GetInstance(className);

        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;

        var callCount = (int)instance.SideEffectBeforeCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }

    [Theory]
    [InlineData([true, "same", "same", 1])]
    [InlineData([true, "different1", "different2", 2])]
    [InlineData([false, "same", "same", 2])]
    [InlineData([false, "different1", "different2", 2])]
    public void CallsPostAssignmentSideEffectLegacy(bool checkForEquality, string firstAssignment, string secondAssignment, int expectedCallCount)
    {
        const string className = "WithSideEffectAfterValueAssignment";

        var weaver = new ModuleWeaver() { CheckForEquality = checkForEquality, EnsureNonInterferenceWithCustomSetterBehaviors = false };
        var testResult = weaver.ExecuteTestRun(assemblyName, ignoreCodes: peVerifyIgnoreCodes);

        var instance = testResult.GetInstance(className);

        instance.Property1 = firstAssignment;
        instance.Property1 = secondAssignment;

        var callCount = (int)instance.SideEffectAfterCallCount;
        Assert.Equal(expectedCallCount, callCount);
    }
}