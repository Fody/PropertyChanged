using System.Collections.Generic;
using System.Linq;

public class CodeGenTypeCleaner
{
    TypeNodeBuilder typeNodeBuilder;

    public CodeGenTypeCleaner(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes.ToList())
        {
            var customAttributes = node.TypeDefinition.CustomAttributes;
            if (customAttributes.ContainsAttribute("System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
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