using System.Collections.Generic;
using System.Linq;

public class DoNotNotifyTypeCleaner
{
    TypeNodeBuilder typeNodeBuilder;

    public DoNotNotifyTypeCleaner(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes.ToList())
        {
            var containsDoNotNotifyAttribute = node.TypeDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotNotifyAttribute");
            if (containsDoNotNotifyAttribute)
            {
                notifyNodes.Remove(node);
                continue;
            }
            Process(node.Nodes);
        }
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}