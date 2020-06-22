using System.Xml.Linq;
using Xunit;

public class EventInvokerNamesConfigTests
{
    [Fact]
    public void GetStringComparisonFromXml()
    {
        var xElement = XElement.Parse("<PropertyChanged EventInvokerNames='A,B'/>");
        var moduleWeaver = new ModuleWeaver
        {
            Config = xElement
        };
        moduleWeaver.ResolveEventInvokerName();

        // Custom values should override the defaults, but the injected method name should always be included

        Assert.Equal(
            new[]
            {
                "A", "B", "<>OnPropertyChanged"
            },
            moduleWeaver.EventInvokerNames);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveEventInvokerName();
        Assert.Contains("OnPropertyChanged", moduleWeaver.EventInvokerNames);
        Assert.Contains("SetProperty", moduleWeaver.EventInvokerNames);
        Assert.Contains("NotifyOfPropertyChange", moduleWeaver.EventInvokerNames);
        Assert.Contains("RaisePropertyChanged", moduleWeaver.EventInvokerNames);
        Assert.Contains("NotifyPropertyChanged", moduleWeaver.EventInvokerNames);
        Assert.Contains("NotifyChanged", moduleWeaver.EventInvokerNames);
        Assert.Contains("ReactiveUI.IReactiveObject.RaisePropertyChanged", moduleWeaver.EventInvokerNames);
        Assert.Contains("<>OnPropertyChanged", moduleWeaver.EventInvokerNames);
    }
}