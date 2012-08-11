using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class AlreadyNotifyFinder
{
    
    public static IEnumerable<string> GetAlreadyNotifies(this PropertyDefinition propertyDefinition, string methodName)
    {
        if (propertyDefinition.SetMethod.IsAbstract)
        {
            yield break;
        }
        var instructions = propertyDefinition.SetMethod.Body.Instructions;
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.IsCallToMethod(methodName))
            {
                var before = instructions[index - 1];
                if (before.OpCode == OpCodes.Ldstr)
                {
                    yield return (string) before.Operand;
                }
            }
        }
    }
}