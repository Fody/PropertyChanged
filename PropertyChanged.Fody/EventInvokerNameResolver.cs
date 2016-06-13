using System;
using System.Collections.Generic;
using System.Linq;

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
        "raisePropertyChanged"
    };


    public void ResolveEventInvokerName()
    {
        var eventInvokerAttribute = Config?.Attributes("EventInvokerNames").FirstOrDefault();
        if (eventInvokerAttribute != null)
        {
            EventInvokerNames.InsertRange(0, eventInvokerAttribute.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length > 0).ToList());
        }
    }
}