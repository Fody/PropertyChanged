using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class RecursiveIlFinder
{
    TypeDefinition typeDefinition;
    List<MethodDefinition> processedMethods;
    public List<Instruction> Instructions;

    public RecursiveIlFinder(TypeDefinition typeDefinition)
    {
        this.typeDefinition = typeDefinition;
        Instructions = new List<Instruction>();
        processedMethods = new List<MethodDefinition>();
    }

    public void Execute(MethodDefinition getMethod)
    {
        processedMethods.Add(getMethod);
        if (getMethod.Body == null)
        {
            return;
        }
        foreach (var instruction in getMethod.Body.Instructions)
        {
            Instructions.Add(instruction);
            if (!IsCall(instruction.OpCode))
            {
                continue;
            }
            var methodDefinition = instruction.Operand as MethodDefinition;
            if (methodDefinition == null)
            {
                continue;
            }
            if (methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                continue;
            }
            if (processedMethods.Contains(methodDefinition))
            {
                continue;
            }
            if (methodDefinition.DeclaringType != typeDefinition)
            {
                continue;
            }
            Execute(methodDefinition);
        }
    }

    static bool IsCall(OpCode opCode)
    {
        return opCode == OpCodes.Call ||
               opCode == OpCodes.Callvirt ||
               opCode == OpCodes.Calli ||
               opCode == OpCodes.Ldftn;
    }

}