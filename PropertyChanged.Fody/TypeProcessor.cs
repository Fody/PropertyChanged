using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public class TypeProcessor
{
    TypeNodeBuilder typeNodeBuilder;
    ModuleWeaver moduleWeaver;
    MsCoreReferenceFinder msCoreReferenceFinder;
    TypeEqualityFinder typeEqualityFinder;

    public TypeProcessor(TypeNodeBuilder typeNodeBuilder, ModuleWeaver moduleWeaver, MsCoreReferenceFinder msCoreReferenceFinder, TypeEqualityFinder typeEqualityFinder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.moduleWeaver = moduleWeaver;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
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
                if (AlreadyContainsNotification(propertyData.PropertyDefinition, node.EventInvoker.MethodReference.Name))
                {
                    moduleWeaver.LogInfo(string.Format("\t{0} Already has notification functionality. Property will be ignored.", propertyData.PropertyDefinition.GetName()));
                    continue;
                }


                var body = propertyData.PropertyDefinition.SetMethod.Body;

                var alreadyHasEquality = HasEqualityChecker.AlreadyHasEquality(propertyData.PropertyDefinition, propertyData.BackingFieldReference);
               
                body.SimplifyMacros();
             
                body.MakeLastStatementReturn();

                var propertyWeaver = new PropertyWeaver(msCoreReferenceFinder, moduleWeaver, propertyData, node, moduleWeaver.ModuleDefinition.TypeSystem);
                propertyWeaver.Execute();

                if (!alreadyHasEquality)
                {
                    var equalityCheckWeaver = new EqualityCheckWeaver(msCoreReferenceFinder, propertyData, typeEqualityFinder);
                    equalityCheckWeaver.Execute();
                }

                body.InitLocals = true;
                body.OptimizeMacros();
            }
            Process(node.Nodes);
        }
    }

    public static bool AlreadyContainsNotification(PropertyDefinition propertyDefinition, string methodName)
    {
        var instructions = propertyDefinition.SetMethod.Body.Instructions;
        return instructions.Any(x =>
                                x.OpCode.IsCall() &&
                                x.Operand is MethodReference &&
                                ((MethodReference) x.Operand).Name == methodName);
    }

}