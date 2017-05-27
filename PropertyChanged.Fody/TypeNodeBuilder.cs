using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;


public partial class ModuleWeaver
{
    List<TypeDefinition> allClasses;
    public List<TypeNode> Nodes;
    public List<TypeNode> NotifyNodes;

    public void BuildTypeNodes()
    {
        // setup a filter delegate to apply the namespace filters
        Func<TypeDefinition, bool> extraFilter =
            t => !NamespaceFilters.Any() || NamespaceFilters.Any(filter => Regex.IsMatch(t.FullName, filter));

        allClasses = ModuleDefinition
            .GetTypes()
            .Where(x => x.IsClass && x.BaseType != null)
            .Where(extraFilter)
            .ToList();
        Nodes = new List<TypeNode>();
        NotifyNodes = new List<TypeNode>();
        TypeDefinition typeDefinition;
        while ((typeDefinition = allClasses.FirstOrDefault()) != null)
        {
            AddClass(typeDefinition);
        }

        PopulateINotifyNodes(Nodes);
        foreach (var notifyNode in NotifyNodes)
        {
            Nodes.Remove(notifyNode);
        }
        PopulateInjectedINotifyNodes(Nodes);
    }

    void PopulateINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            if (HierarchyImplementsINotify(node.TypeDefinition))
            {
                NotifyNodes.Add(node);
                continue;
            }
            PopulateINotifyNodes(node.Nodes);
        }
    }

    void PopulateInjectedINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            PopulateInjectedINotifyNodes(node.Nodes);
        }
    }

    TypeNode AddClass(TypeDefinition typeDefinition)
    {
        allClasses.Remove(typeDefinition);
        var typeNode = new TypeNode
        {
            TypeDefinition = typeDefinition
        };
        if (typeDefinition.BaseType.Scope.Name != ModuleDefinition.Name)
        {
            Nodes.Add(typeNode);
        }
        else
        {
            var baseType = Resolve(typeDefinition.BaseType);
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