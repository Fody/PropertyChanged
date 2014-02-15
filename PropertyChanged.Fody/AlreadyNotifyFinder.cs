using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    
    public IEnumerable<string> GetAlreadyNotifies(PropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.SetMethod.IsAbstract)
        {
            yield break;
        }
        var instructions = propertyDefinition.SetMethod.Body.Instructions;
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            foreach (var methodName in EventInvokerNames)
            {

                int propertyNameIndex;
                if (instruction.IsCallToMethod(methodName, out propertyNameIndex))
                {
                    var before = instructions[index - propertyNameIndex];
                    if (before.OpCode == OpCodes.Ldstr)
                    {
                        yield return (string) before.Operand;
                    }
                }
            }
        }
    }
}