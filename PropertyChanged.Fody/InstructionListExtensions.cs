using System.Linq;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class InstructionListExtensions
{
    public static void Prepend(this Collection<Instruction> collection, params Instruction[] instructions)
    {
        for (var index = 0; index < instructions.Length; index++)
        {
            var instruction = instructions[index];
            collection.Insert(index, instruction);
        }
    }

    public static void Append(this Collection<Instruction> collection, params Instruction[] instructions)
    {
        for (var index = 0; index < instructions.Length; index++)
        {
            collection.Insert(index, instructions[index]);
        }
    }

    public static int Insert(this Collection<Instruction> collection, int index, params Instruction[] instructions)
    {
        foreach (var instruction in instructions.Reverse())
        {
            collection.Insert(index, instruction);
        }
        return index + instructions.Length;
    }
}