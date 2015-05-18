using System.Collections.Generic;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    public void ProcessTypes()
    {
        ProcessTypes(NotifyNodes);
    }

    void ProcessTypes(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            if (node.EventInvoker == null)
            {
                continue;
            }

            LogDebug("\t" + node.TypeDefinition.FullName);

            foreach (var propertyData in node.PropertyDatas)
            {
                var body = propertyData.PropertyDefinition.SetMethod.Body;

                var alreadyHasEquality = HasEqualityChecker.AlreadyHasEquality(propertyData.PropertyDefinition, propertyData.BackingFieldReference);
               
                body.SimplifyMacros();
             
                body.MakeLastStatementReturn();

                var propertyWeaver = new PropertyWeaver(this, propertyData, node, ModuleDefinition.TypeSystem);
                propertyWeaver.Execute();

                if (!alreadyHasEquality)
                {
                    var equalityCheckWeaver = new EqualityCheckWeaver(propertyData, node.TypeDefinition, this);
                    equalityCheckWeaver.Execute();
                }

                body.InitLocals = true;
                body.OptimizeMacros();
            }

            ProcessTypes(node.Nodes);
        }
    }
}