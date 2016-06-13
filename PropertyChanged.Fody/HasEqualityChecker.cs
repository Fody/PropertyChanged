using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class HasEqualityChecker
{
    public static bool AlreadyHasEquality(PropertyDefinition propertyDefinition, FieldReference backingFieldReference)
    {
        var instructions = propertyDefinition.SetMethod.Body.Instructions;
        var list = instructions.Where(IsNotNop).ToList();
        if (list.Count < 4)
        {
            return false;
        }
        var firstFive = list.Take(5).ToList();
        if (firstFive.All(x => x.OpCode != OpCodes.Ldarg_1))
        {
            return false;
        }
        if (firstFive.All(x => x.OpCode != OpCodes.Ldarg_0))
        {
            return false;
        }
        if (firstFive.All(x => !x.IsEquality()))
        {
            return false;
        }

        if (firstFive.Any(x => x.Operand == backingFieldReference))
        {
            return true;
        }
        if (firstFive.Any(x => x.Operand == propertyDefinition))
        {
            return true;
        }
        return false;
    }

    static bool IsEquality(this Instruction instruction)
    {
        var opCode = instruction.OpCode;
        if (opCode == OpCodes.Ceq ||
            opCode == OpCodes.Beq_S ||
            opCode == OpCodes.Bne_Un ||
            opCode == OpCodes.Bne_Un_S ||
            opCode == OpCodes.Beq)
        {
            return true;
        }
        var memberReference = instruction.Operand as MemberReference;
        if (memberReference != null)
        {
            return memberReference.Name == "Equals" || memberReference.Name == "op_Inequality" || memberReference.Name == "op_Equality";
        }

        return false;
    }

    static bool IsNotNop(this Instruction instruction)
    {
        return instruction.OpCode != OpCodes.Nop;
    }
}