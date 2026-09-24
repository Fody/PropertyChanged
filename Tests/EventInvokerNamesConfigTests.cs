using System.Xml.Linq;

public class EventInvokerNamesConfigTests
{
    [Test]
    public async Task GetStringComparisonFromXml()
    {
        var xElement = XElement.Parse("<PropertyChanged EventInvokerNames='A,B'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveEventInvokerName();

        // Custom values should override the defaults, but the injected method name should always be included

        await Assert.That(weaver.EventInvokerNames).IsEquivalentTo(new[]
            {
                "A", "B", "<>OnPropertyChanged"
            });
    }

    [Test]
    public async Task Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveEventInvokerName();
        await Assert.That(weaver.EventInvokerNames).Contains("OnPropertyChanged");
        await Assert.That(weaver.EventInvokerNames).Contains("SetProperty");
        await Assert.That(weaver.EventInvokerNames).Contains("NotifyOfPropertyChange");
        await Assert.That(weaver.EventInvokerNames).Contains("RaisePropertyChanged");
        await Assert.That(weaver.EventInvokerNames).Contains("NotifyPropertyChanged");
        await Assert.That(weaver.EventInvokerNames).Contains("NotifyChanged");
        await Assert.That(weaver.EventInvokerNames).Contains("ReactiveUI.IReactiveObject.RaisePropertyChanged");
        await Assert.That(weaver.EventInvokerNames).Contains("<>OnPropertyChanged");
    }
}