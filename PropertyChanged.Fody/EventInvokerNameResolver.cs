using System;
using System.Collections.Generic;
using System.Linq;

public class EventInvokerNameResolver
{
    ModuleWeaver moduleWeaver;
    public List<string> EventInvokerNames { get; set; }

 
    public EventInvokerNameResolver(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
        EventInvokerNames = new List<string> { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged", "NotifyPropertyChanged", "NotifyChanged" };
    }

    public void Execute()
    {
        if (moduleWeaver.Config != null)
        {
            var eventInvokerAttribute = moduleWeaver.Config.Attributes("EventInvokerNames").FirstOrDefault();
            if (eventInvokerAttribute != null)
            {
                EventInvokerNames.InsertRange(0,eventInvokerAttribute.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x=>x.Length > 0).ToList());
            }
        }
    }
}