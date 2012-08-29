using System.ComponentModel;
using NUnit.Framework;

[TestFixture]
public class ExperimentTests
{
    [Test]
    [Ignore]
    public void Foo()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyExperiments\AssemblyExperiments.csproj");

        var instance = weaverHelper.Assembly.GetInstance("ExperimentClass");

        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 1;

        Assert.IsTrue(property1EventCalled);
    }
}