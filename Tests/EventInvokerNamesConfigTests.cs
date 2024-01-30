using System.Xml.Linq;

public class EventInvokerNamesConfigTests
{
    [Fact]
    public void GetStringComparisonFromXml()
    {
        var xElement = XElement.Parse("<PropertyChanged EventInvokerNames='A,B'/>");
        var weaver = new ModuleWeaver
        {
            Config = xElement
        };
        weaver.ResolveEventInvokerName();

        // Custom values should override the defaults, but the injected method name should always be included

        Assert.Equal(
            new[]
            {
                "A", "B", "<>OnPropertyChanged"
            },
            weaver.EventInvokerNames);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveEventInvokerName();
        Assert.Contains("OnPropertyChanged", weaver.EventInvokerNames);
        Assert.Contains("SetProperty", weaver.EventInvokerNames);
        Assert.Contains("NotifyOfPropertyChange", weaver.EventInvokerNames);
        Assert.Contains("RaisePropertyChanged", weaver.EventInvokerNames);
        Assert.Contains("NotifyPropertyChanged", weaver.EventInvokerNames);
        Assert.Contains("NotifyChanged", weaver.EventInvokerNames);
        Assert.Contains("ReactiveUI.IReactiveObject.RaisePropertyChanged", weaver.EventInvokerNames);
        Assert.Contains("<>OnPropertyChanged", weaver.EventInvokerNames);
    }
}