using System.Collections.Generic;
using System.Linq;

public partial class ModuleWeaver
{
    void ProcessNotifyNodes(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes.ToList())
        {
            var customAttributes = node.TypeDefinition.CustomAttributes;
            if (customAttributes.ContainsAttribute("System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
            {
                notifyNodes.Remove(node);
                continue;
            }
            ProcessNotifyNodes(node.Nodes);
        }
    }

    public void CleanCodeGenedTypes()
    {
        ProcessNotifyNodes(NotifyNodes);
    }
}