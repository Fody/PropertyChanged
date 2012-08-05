using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;


public class TypeNodeBuilder
{
    ModuleWeaver moduleWeaver;
    NotifyInterfaceFinder notifyInterfaceFinder;
    TypeResolver typeResolver;
    List<TypeDefinition> allClasses;
    public List<TypeNode> Nodes;
    public List<TypeNode> NotifyNodes;
    ModuleDefinition moduleDefinition;

    public TypeNodeBuilder(ModuleWeaver moduleWeaver, NotifyInterfaceFinder notifyInterfaceFinder, TypeResolver typeResolver, List<TypeDefinition> allTypesFinder)
    {
        this.moduleWeaver = moduleWeaver;
        this.notifyInterfaceFinder = notifyInterfaceFinder;
        this.typeResolver = typeResolver;
        allClasses = allTypesFinder.Where(x => x.IsClass).ToList();
    }

    public void Execute()
    {
        moduleDefinition = moduleWeaver.ModuleDefinition;
        Nodes = new List<TypeNode>();
        NotifyNodes = new List<TypeNode>();
        TypeDefinition typeDefinition;
        while ((typeDefinition = allClasses.FirstOrDefault()) != null)
        {
            AddClass(typeDefinition);
        }
        var typeNodes = Nodes;

        PopulateINotifyNodes(typeNodes);
    }

    void PopulateINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            if (notifyInterfaceFinder.HierachyImplementsINotify(node.TypeDefinition))
            {
                NotifyNodes.Add(node);
                continue;
            }
            PopulateINotifyNodes(node.Nodes);
        }
    }

    TypeNode AddClass(TypeDefinition typeDefinition)
    {
        allClasses.Remove(typeDefinition);
        var typeNode = new TypeNode
                           {
                               TypeDefinition = typeDefinition
                           };
        if (typeDefinition.BaseType.Scope.Name != moduleDefinition.Name)
        {
            Nodes.Add(typeNode);
        }
        else
        {
            var baseType = typeResolver.Resolve(typeDefinition.BaseType);
            var parentNode = FindClassNode(baseType, Nodes);
            if (parentNode == null)
            {
                parentNode = AddClass(baseType);
            }
            parentNode.Nodes.Add(typeNode);
        }
        return typeNode;

    }

    TypeNode FindClassNode(TypeDefinition type, IEnumerable<TypeNode> typeNode)
    {
        foreach (var node in typeNode)
        {
            if (type == node.TypeDefinition)
            {
                return node;
            }
            var findNode = FindClassNode(type, node.Nodes);
            if (findNode != null)
            {
                return findNode;
            }
        }
        return null;
    }

}