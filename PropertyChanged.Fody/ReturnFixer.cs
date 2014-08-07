using Mono.Cecil.Cil;

public static class ReturnFixer
{
    public static void MakeLastStatementReturn(this MethodBody method)
    {
        var instructions = method.Instructions;

        // Method is just return, do nothing
        if (instructions.Count == 1)
            return;

        var count = instructions.Count - 1;
        var last = instructions[count];
        Instruction secondLastInstruction;

        if (last.OpCode == OpCodes.Ret)
        {
            count--;
            secondLastInstruction = method.Instructions[count];
            if (secondLastInstruction.OpCode != OpCodes.Nop)
            {
                secondLastInstruction = Instruction.Create(OpCodes.Nop);
                instructions.BeforeLast(secondLastInstruction);
            }
        }
        else
        {
            secondLastInstruction = Instruction.Create(OpCodes.Nop);
            instructions.Add(secondLastInstruction);
            instructions.Add(Instruction.Create(OpCodes.Ret));
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
}