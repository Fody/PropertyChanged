using System.Collections.Generic;
using System.Linq;

public class WarningChecker
{
    TypeNodeBuilder typeNodeBuilder;
    ModuleWeaver moduleWeaver;


    public WarningChecker(TypeNodeBuilder typeNodeBuilder, ModuleWeaver moduleWeaver)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.moduleWeaver = moduleWeaver;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var propertyData in node.PropertyDatas.ToList())
            {
                var warning = CheckForWarning(propertyData, node.EventInvoker.IsBeforeAfter);
                if (warning != null)
                {
                    moduleWeaver.LogInfo(string.Format("\t{0} {1} Property will be ignored.", propertyData.PropertyDefinition.GetName(), warning));
                    node.PropertyDatas.Remove(propertyData);
                }
            }
            Process(node.Nodes);
        }
    }

    public string CheckForWarning(PropertyData propertyData, bool isBeforeAfter)
    {
        var propertyDefinition = propertyData.PropertyDefinition;
        var setMethod = propertyDefinition.SetMethod;
        if (setMethod.Name == "set_Item" && setMethod.Parameters.Count == 2 && setMethod.Parameters[1].Name == "value")
        {
            return "Property is an indexer.";
        }
        if (setMethod.IsAbstract)
        {
            return "Property is abstract.";
        }
        if ((propertyData.BackingFieldReference == null) && (propertyDefinition.GetMethod == null))
        {
            return "Property has no field set logic or it contains multiple sets and the names cannot be mapped to a property.";
        }
        if (isBeforeAfter && (propertyDefinition.GetMethod == null))
        {
            return "When using a before/after invoker the property have a 'get'.";
        }
        return null;
    }


    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}