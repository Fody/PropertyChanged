using System.Collections.Generic;
using Mono.Cecil.Cil;
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

            WriteDebug("\t" + node.TypeDefinition.FullName);

            foreach (var propertyData in node.PropertyDatas)
            {
                var body = propertyData.PropertyDefinition.SetMethod.Body;
                var alreadyHasEquality = HasEqualityChecker.AlreadyHasEquality(propertyData.PropertyDefinition, propertyData.BackingFieldReference);

                body.SimplifyMacros();
                body.MakeLastStatementReturn();

                var resultEquals = new VariableDefinition(TypeSystem.BooleanReference);
                propertyData.PropertyDefinition.SetMethod.Body.Variables.Add(resultEquals);

                if (!alreadyHasEquality)
                {
                    var equalityCheckWeaver = new EqualityCheckWeaver(propertyData, node.TypeDefinition, this, resultEquals);
                    equalityCheckWeaver.Execute();
                }

                var propertyWeaver = new PropertyWeaver(this, propertyData, node, TypeSystem, resultEquals);
                propertyWeaver.Execute();

                body.InitLocals = true;
                body.OptimizeMacros();
            }

            ProcessTypes(node.Nodes);
        }
    }
}