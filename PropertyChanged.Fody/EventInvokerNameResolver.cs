using System;
using System.Collections.Generic;
using System.Linq;
using Fody;

public partial class ModuleWeaver
{
    public List<string> EventInvokerNames = new List<string>
    {
        "OnPropertyChanged",
        "SetProperty",
        "NotifyOfPropertyChange",
        "RaisePropertyChanged",
        "NotifyPropertyChanged",
        "NotifyChanged",
        "raisePropertyChanged",
        injectedEventInvokerName
    };

    public void ResolveEventInvokerName()
    {
        var eventInvokerAttribute = Config?.Attributes("EventInvokerNames").FirstOrDefault();
        if (eventInvokerAttribute == null)
        {
            return;
        }

        EventInvokerNames = eventInvokerAttribute.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();

        if (!EventInvokerNames.Any())
        {
            throw new WeavingException("EventInvokerNames contained no items.");
        }

        EventInvokerNames.Add(injectedEventInvokerName);
    }
}