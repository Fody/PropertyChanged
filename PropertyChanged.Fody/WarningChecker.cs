using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

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
                    EmitConditionalWarning(propertyData.PropertyDefinition, $"{propertyData.PropertyDefinition.GetName()} {warning} Property will be ignored.");
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

    public void EmitWarning(string message)
    {
        if (SuppressWarnings)
            return;
        
        LogWarning?.Invoke(message);
    }
    
    public void EmitConditionalWarning(ICustomAttributeProvider member, string message)
    {
        if (SuppressWarnings)
            return;

        var suppressAttrName = "PropertyChanged.SuppressPropertyChangedWarningsAttribute";

        if (member.HasCustomAttributes && member.CustomAttributes.ContainsAttribute(suppressAttrName))
            return;

        if (member is IMemberDefinition memberDefinition && memberDefinition.DeclaringType.GetAllCustomAttributes().ContainsAttribute(suppressAttrName))
            return;

        if (member is MethodDefinition method)
        {
            // Get the first sequence point of the method to get an approximate location for the warning 
            var sequencePoint = method.DebugInformation.HasSequencePoints
                ? method.DebugInformation.SequencePoints.FirstOrDefault()
                : null;
        
            if (!message.EndsWith("."))
                message += ".";
            
            LogWarningPoint?.Invoke($"{message} You can suppress this warning with [SuppressPropertyChangedWarnings].", sequencePoint);
        }
    }
}
