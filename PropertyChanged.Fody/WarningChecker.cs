using System.Collections.Generic;
using System.Linq;

public partial class ModuleWeaver
{
    void CheckForWarnings(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var propertyData in node.PropertyDatas.ToList())
            {
                var warning = CheckForWarning(propertyData, node.EventInvoker.InvokerType);
                if (warning != null)
                {
                    LogDebug($"\t{propertyData.PropertyDefinition.GetName()} {warning} Property will be ignored.");
                    node.PropertyDatas.Remove(propertyData);
                }
            }
            CheckForWarnings(node.Nodes);
        }
    }

    public string CheckForWarning(PropertyData propertyData, InvokerTypes invokerType)
    {
        var propertyDefinition = propertyData.PropertyDefinition;
        var setMethod = propertyDefinition.SetMethod;

        if (setMethod.Name == "set_Item" && setMethod.Parameters.Count == 2 && setMethod.Parameters[1].Name == "value")
        {
            return "Property is an indexer.";
        }
        if (setMethod.Parameters.Count > 1)
        {
            return "Property takes more than one parameter.";
        }
        if (setMethod.IsAbstract)
        {
            return "Property is abstract.";
        }
        if (propertyData.BackingFieldReference == null && propertyDefinition.GetMethod == null)
        {
            return "Property has no field set logic or it contains multiple sets and the names cannot be mapped to a property.";
        }
        if (invokerType == InvokerTypes.BeforeAfter && propertyDefinition.GetMethod == null)
        {
            return "When using a before/after invoker the property have a 'get'.";
        }
        return null;
    }


    public void CheckForWarnings()
    {
        CheckForWarnings(NotifyNodes);
    }
}