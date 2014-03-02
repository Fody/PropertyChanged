using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    void FindAllProperties(List<TypeNode> notifyNodes, List<PropertyDefinition> list)
    {
        foreach (var node in notifyNodes)
        {
            var properties = node.TypeDefinition.Properties.ToList();
            properties.AddRange(list);
            node.AllProperties = properties;
            FindAllProperties(node.Nodes, properties);
        }
    }

    public void FindAllProperties()
    {
        FindAllProperties(NotifyNodes, new List<PropertyDefinition>());
    }
}