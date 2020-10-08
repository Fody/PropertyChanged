namespace Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Fody;

    using Xunit;
    using Xunit.Abstractions;

    public class AssemblyWithInheritanceTests
    {
        readonly ITestOutputHelper outputHelper;

        static AssemblyWithInheritanceTests()
        {
            var weavingTask = new ModuleWeaver();

            testResults = new[]
            {
                weavingTask.ExecuteTestRun("AssemblyWithInheritance.dll"),
                weavingTask.ExecuteTestRun("AssemblyWithExternalInheritance.dll"),
            };
        }

        public AssemblyWithInheritanceTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Theory]
        [InlineData(0, "DerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed" })]
        [InlineData(0, "DerivedClass", "Property2", new[] { "Property5", "derived:OnProperty2Changed", "Property2" })]
        [InlineData(0, "DerivedClass", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
        [InlineData(0, "DerivedNoOverrides", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1" })]
        [InlineData(0, "DerivedNoOverrides", "Property2", new[] { "Property4", "base:OnProperty2Changed", "Property2" })]
        [InlineData(0, "DerivedNoOverrides", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
        [InlineData(0, "DerivedDerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed", "Property6", "derived++:OnProperty1Changed" })]
        [InlineData(0, "DerivedDerivedClass", "Property2", new[] { "Property6", "derived++:OnProperty2Changed", "Property2" })]
        [InlineData(0, "DerivedDerivedClass", "Property3", new[] { "Property6", "derived++:OnProperty3Changed", "Property3" })]
        // TODO: [InlineData(1, "DerivedClass", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1", "Property5", "derived:OnProperty1Changed" })]
        // TODO: [InlineData(1, "DerivedClass", "Property2", new[] { "Property5", "derived:OnProperty2Changed", "Property2" })]
        [InlineData(1, "DerivedClass", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
        // TODO: [InlineData(1, "DerivedNoOverrides", "Property1", new[] { "Property4", "base:OnProperty1Changed", "Property1" })]
        // TODO: [InlineData(1, "DerivedNoOverrides", "Property2", new[] { "Property4", "base:OnProperty2Changed", "Property2" })]
        [InlineData(1, "DerivedNoOverrides", "Property3", new[] { "Property5", "derived:OnProperty3Changed", "Property3" })]
        public void DerivedClassRaisesAllExpectedEvents(int assemblyIndex, string className, string propertyName, string[] expected)
        {
            var instance = testResults[assemblyIndex].GetInstance(className);
            var actual = (IList<string>)instance.Notifications;

            ((object)instance).GetType().GetProperty(propertyName)?.SetValue(instance, 42);

            outputHelper.WriteLine(string.Join(", ", actual.Select(item => $"\"{item}\"")));

            Assert.Equal(expected, actual);
        }

        static TestResult[] testResults;
    }
}