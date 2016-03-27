using System.Linq;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class ReturnFixer
{
    public static void MakeLastStatementReturn(this MethodBody method)
    {
        var instructions = method.Instructions;

        // Method is just return, do nothing
        if (instructions.Count == 1)
        {
            return;
        }

        var last = GetLast(instructions);

        var secondLastInstruction = last.Previous;
        if (secondLastInstruction.OpCode != OpCodes.Nop)
        {
            secondLastInstruction = Instruction.Create(OpCodes.Nop);
            var indexOf = instructions.IndexOf(last);
            instructions.Insert(indexOf, secondLastInstruction);
        }

        foreach (var exceptionHandler in method.ExceptionHandlers)
        {
            if (exceptionHandler.HandlerEnd == last)
            {
                exceptionHandler.HandlerEnd = secondLastInstruction;
            }
        }
        foreach (var instruction in instructions)
        {
            if (instruction == secondLastInstruction)
            {
                break;
            }
            if (instruction.OpCode == OpCodes.Ret)
            {
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = secondLastInstruction;
                continue;
            }
            if (instruction.Operand == last)
            {
                instruction.Operand = secondLastInstruction;
            }
        }
    }

    static Instruction GetLast(Collection<Instruction> instructions)
    {
        var tail = instructions.LastOrDefault(x => x.OpCode == OpCodes.Tail);
        if (tail != null)
        {
            return tail;
        }
        var last = instructions.Last();
        if (last.OpCode == OpCodes.Ret)
        {
            return last;
        }
        last = Instruction.Create(OpCodes.Ret);
        instructions.Add(last);
        return last;
    }
}