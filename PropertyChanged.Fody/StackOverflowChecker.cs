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
                if (node.EventInvoker.InvokerType == InvokerTypes.BeforeAfter)
                {
                    if (CheckIfGetterCallsSetter(propertyData.PropertyDefinition))
                    {
                        throw new WeavingException(string.Format("{0} Getter calls setter which will cause a stack overflow as the setter uses the getter for obtaining the before and after values.", propertyData.PropertyDefinition.GetName()));
                    }

                    if (CheckIfGetterCallsVirtualBaseSetter(propertyData.PropertyDefinition))
                    {
                        throw new WeavingException(string.Format("{0} Getter of calls virtual setter of base class which will cause a stack overflow as the setter uses the getter for obtaining the before and after values.", propertyData.PropertyDefinition.GetName()));
                    }
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

            if (baseProperty != null)
            {
                if (propertyDefinition.GetMethod != null)
                {
                    var instructions = propertyDefinition.GetMethod.Body.Instructions;
                    foreach (var instruction in instructions)
                    {
                        if (instruction.OpCode == OpCodes.Call
                            && instruction.Operand is MethodReference
                            && ((MethodReference) instruction.Operand).Resolve() == baseProperty.SetMethod)
                        {
                            return true;
                        }
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