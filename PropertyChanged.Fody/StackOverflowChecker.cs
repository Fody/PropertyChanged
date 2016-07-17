using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;


public partial class ModuleWeaver
{

    void CheckForStackOverflow(IEnumerable<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var propertyData in node.PropertyDatas)
            {
                if (node.EventInvoker.InvokerType != InvokerTypes.BeforeAfter)
                {
                    continue;
                }
                if (CheckIfGetterCallsSetter(propertyData.PropertyDefinition))
                {
                    throw new WeavingException($"{propertyData.PropertyDefinition.GetName()} Getter calls setter which will cause a stack overflow as the setter uses the getter for obtaining the before and after values.");
                }

                if (CheckIfGetterCallsVirtualBaseSetter(propertyData.PropertyDefinition))
                {
                    throw new WeavingException($"{propertyData.PropertyDefinition.GetName()} Getter of calls virtual setter of base class which will cause a stack overflow as the setter uses the getter for obtaining the before and after values.");
                }
            }

            CheckForStackOverflow(node.Nodes);
        }
    }

    public bool CheckIfGetterCallsSetter(PropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.GetMethod != null)
        {
            var instructions = propertyDefinition.GetMethod.Body.Instructions;
            foreach (var instruction in instructions)
            {
                if (instruction.OpCode == OpCodes.Call
                    && instruction.Operand == propertyDefinition.SetMethod)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckIfGetterCallsVirtualBaseSetter(PropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.SetMethod.IsVirtual)
        {
            var baseType = Resolve(propertyDefinition.DeclaringType.BaseType);
            var baseProperty = baseType.Properties.FirstOrDefault(x => x.Name == propertyDefinition.Name);

            if (baseProperty != null && propertyDefinition.GetMethod != null)
            {
                var instructions = propertyDefinition.GetMethod.Body.Instructions;
                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode != OpCodes.Call)
                    {
                        continue;
                    }
                    var operand = instruction.Operand as MethodReference;
                    if (operand == null)
                    {
                        continue;
                    }
                    if (operand.FullName == baseProperty.SetMethod.FullName)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void CheckForStackOverflow()
    {
        CheckForStackOverflow(NotifyNodes);
    }
}