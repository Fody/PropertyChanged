using System.Linq;
using Mono.Cecil.Cil;

public static class ReturnFixer
{
    public static void MakeLastStatementReturn(this MethodBody method)
    {
        var instructions = method.Instructions;
        var last = instructions.Last();

        var count = method.Instructions.Count;
        count--;
        var secondLastInstruction = method.Instructions[count];

        if (secondLastInstruction.OpCode != OpCodes.Nop)
        {
            secondLastInstruction = Instruction.Create(OpCodes.Nop);
            instructions.BeforeLast(secondLastInstruction);
        }
        else
        {
            count--;
        }


        foreach (var exceptionHandler in method.ExceptionHandlers)
        {
            if (exceptionHandler.HandlerEnd == last)
            {
                exceptionHandler.HandlerEnd = secondLastInstruction;
            }
        }
        for (var index = 0; index < count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Ret)
            {
                instructions[index] = Instruction.Create(OpCodes.Br, secondLastInstruction);
            }
            if (instruction.Operand == last)
            {
                instructions[index] = Instruction.Create(instruction.OpCode, secondLastInstruction);
            }
        }
    }
}