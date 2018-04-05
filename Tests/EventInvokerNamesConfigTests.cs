using System.Xml.Linq;
using Xunit;

public class EventInvokerNamesConfigTests
{
    [Fact]
    public void GetStringComparisonFromXml()
    {
        var xElement = XElement.Parse("<PropertyChanged EventInvokerNames='A,B'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveEventInvokerName();
        Assert.Contains("A", moduleWeaver.EventInvokerNames);
        Assert.Contains("B", moduleWeaver.EventInvokerNames);

        // Custom values should override the defaults
        Assert.DoesNotContain("OnPropertyChanged", moduleWeaver.EventInvokerNames);
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
        Assert.Contains("raisePropertyChanged", moduleWeaver.EventInvokerNames);
    }
}
