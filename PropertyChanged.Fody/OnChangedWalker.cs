using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void ProcessOnChangedMethods()
    {
        ProcessOnChangedMethods(NotifyNodes);
    }

    void ProcessOnChangedMethods(List<TypeNode> notifyNodes)
    {
        foreach (var notifyNode in notifyNodes)
        {
            notifyNode.OnChangedMethods = GetOnChangedMethods(notifyNode).ToList();
            ProcessOnChangedMethods(notifyNode.Nodes);
        }
    }

    IEnumerable<MethodReference> GetOnChangedMethods(TypeNode notifyNode)
    {
        var methods = notifyNode.TypeDefinition.Methods;

        return methods.Where(x => !x.IsStatic &&
                                  !x.IsAbstract &&
                                  x.Parameters.Count == 0 &&
                                  x.Name.StartsWith("On") &&
                                  x.Name.EndsWith("Changed"))
            .Select(methodDefinition =>
            {
                var typeDefinitions = new Stack<TypeDefinition>();
                typeDefinitions.Push(notifyNode.TypeDefinition);

                return GetMethodReference(typeDefinitions, methodDefinition);
            });
    }
}