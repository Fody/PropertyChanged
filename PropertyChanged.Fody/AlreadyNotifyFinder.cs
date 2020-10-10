using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public IEnumerable<string> GetAlreadyNotifies(PropertyDefinition propertyDefinition)
    {
        var setMethod = propertyDefinition.SetMethod;
        if (setMethod.IsAbstract)
        {
            yield break;
        }

        var instructions = setMethod.Body.Instructions;

        if (setMethod.GetBaseMethod(out var baseMethod))
        {
            if (baseMethod.HasBody && HierarchyImplementsINotify(baseMethod.DeclaringType) && instructions.Any(instruction => instruction.IsCallToMethod(baseMethod)) )
            {
                yield return propertyDefinition.Name;
            }
        }

        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            foreach (var methodName in EventInvokerNames)
            {
                if (!instruction.IsCallToMethod(methodName, out var propertyNameIndex))
                {
                    continue;
                }
                var before = instructions[index - propertyNameIndex];
                if (before.OpCode == OpCodes.Ldstr)
                {
                    yield return (string)before.Operand;
                }
            }
        }
    }
}