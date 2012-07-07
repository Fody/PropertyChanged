using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class AllPropertiesFinder
{
    TypeNodeBuilder typeNodeBuilder;

    public AllPropertiesFinder(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes, List<PropertyDefinition> list)
    {
        foreach (var node in notifyNodes)
        {
            var properties = node.TypeDefinition.Properties.ToList();
            properties.AddRange(list);
            node.AllProperties = properties;
            Process(node.Nodes, properties);
        }
    }


    public void Execute()
    {
        var notifyNodes = typeNodeBuilder.NotifyNodes;
        Process(notifyNodes, new List<PropertyDefinition>());
    }
}