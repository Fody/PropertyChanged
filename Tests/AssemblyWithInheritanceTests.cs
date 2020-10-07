namespace Tests
{
    using System.Collections.Generic;

    using Fody;

    using Xunit;

    public class AssemblyWithInheritanceTests
    {
        static AssemblyWithInheritanceTests()
        {
            var weavingTask = new ModuleWeaver();
            testResult = weavingTask.ExecuteTestRun("AssemblyWithInheritance.dll");
        }

        [Fact]
        public void DerivedClassRaisesAllExpectedEventsAndNoDuplicates()
        {
            var instance = testResult.GetInstance("DerivedClass");
            instance.Property1 = 42;

            var expected = new[] { "Property2", "Property1", "Property3" };

            Assert.Equal(expected, (IList<string>)instance.ChangedEvents);
        }

        static TestResult testResult;
    }
}