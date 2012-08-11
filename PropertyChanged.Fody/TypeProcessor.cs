using System.Collections.Generic;
using Mono.Cecil.Rocks;

public class TypeProcessor
{
    TypeNodeBuilder typeNodeBuilder;
    ModuleWeaver moduleWeaver;
    TypeEqualityFinder typeEqualityFinder;

    public TypeProcessor(TypeNodeBuilder typeNodeBuilder, ModuleWeaver moduleWeaver, TypeEqualityFinder typeEqualityFinder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.moduleWeaver = moduleWeaver;
        this.typeEqualityFinder = typeEqualityFinder;
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            if (node.EventInvoker == null)
            {
                continue;
            }
            moduleWeaver.LogInfo("\t" + node.TypeDefinition.FullName);

            foreach (var propertyData in node.PropertyDatas)
            {
              


                var body = propertyData.PropertyDefinition.SetMethod.Body;

                var alreadyHasEquality = HasEqualityChecker.AlreadyHasEquality(propertyData.PropertyDefinition, propertyData.BackingFieldReference);
               
                body.SimplifyMacros();
             
                body.MakeLastStatementReturn();

                var propertyWeaver = new PropertyWeaver(moduleWeaver, propertyData, node, moduleWeaver.ModuleDefinition.TypeSystem);
                propertyWeaver.Execute();

                if (!alreadyHasEquality)
                {
                    var equalityCheckWeaver = new EqualityCheckWeaver(propertyData, typeEqualityFinder);
                    equalityCheckWeaver.Execute();
                }

                body.InitLocals = true;
                body.OptimizeMacros();
            }
            Process(node.Nodes);
        }
    }


}