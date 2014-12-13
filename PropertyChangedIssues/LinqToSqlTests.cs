using NUnit.Framework;

namespace PropertyChangedIssues
{
    [TestFixture]
    public class LinqToSqlTests
    {
        [Test]
        public void ShouldRaisesNotifyPropertyChangedOnce()
        {
            Class1 c = new Class1();

            int count = 0;
            c.PropertyChanged += (sender, args) => count++;
            c.Property1 = "hello";

            Assert.AreEqual(1, count);
        }

        [Test]
        public void ShouldCallPartialMethodOnce()
        {
            Class1 c = new Class1();
            
            c.Property1 = "hello";
            c.Id = "id";

            Assert.AreEqual(1, c.CalledProperty1OnChangedMethod);
            Assert.AreEqual(1, c.CalledIdOnChangedMethod);
        }
    }
}