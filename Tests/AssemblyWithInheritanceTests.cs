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
        public void DerivedClassRaisesAllExpectedEventsForProperty1AndNoDuplicates()
        {
            var instance = testResult.GetInstance("DerivedClass");
            var actual = (IList<string>)instance.Notifications;

            instance.Property1 = 42;

            var expected = new[] { "Property2", "base:OnProperty1Changed", "Property1", "Property3", "derived:OnProperty1Changed" };

            Assert.Equal(expected, actual);
        }
        [Fact]
        public void DerivedClassRaisesAllExpectedEventsForProperty4AndNoDuplicates()
        {
            var instance = testResult.GetInstance("DerivedClass");
            var actual = (IList<string>)instance.Notifications;

            instance.Property4 = 42;

            var expected = new[] { "Property3", "derived:OnProperty4Changed", "Property4" };

            Assert.Equal(expected, actual);
        }
        [Fact]
        public void DerivedClassRaisesAllExpectedEventsForProperty5AndNoDuplicates()
        {
            var instance = testResult.GetInstance("DerivedClass");
            var actual = (IList<string>)instance.Notifications;

            instance.Property5 = 42;

            var expected = new[] { "Property3", "derived:OnProperty5Changed", "Property5" };

            Assert.Equal(expected, actual);
        }

        static TestResult testResult;
    }
}