using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class EqualityCheckWeaver
{
    PropertyData propertyData;
    TypeEqualityFinder typeEqualityFinder;
    Collection<Instruction> instructions;

    public EqualityCheckWeaver(PropertyData propertyData, TypeEqualityFinder typeEqualityFinder)
    {
        this.propertyData = propertyData;
        this.typeEqualityFinder = typeEqualityFinder;
    }

    public void Execute()
    {
        var property = propertyData.PropertyDefinition;
        instructions = property.SetMethod.Body.Instructions;

        if (propertyData.BackingFieldReference == null)
        {
            CheckAgainstProperty();
        }
        else
        {
            CheckAgainstField();
        }
    }


    void CheckAgainstField()
    {
        var fieldReference = propertyData.BackingFieldReference.Resolve().GetGeneric();
        InjectEqualityCheck(Instruction.Create(OpCodes.Ldfld, fieldReference), fieldReference.FieldType);
    }


    void CheckAgainstProperty()
    {
        var propertyReference = propertyData.PropertyDefinition;
        var methodDefinition = propertyData.PropertyDefinition.GetMethod.GetGeneric();
        InjectEqualityCheck(Instruction.Create(OpCodes.Call, methodDefinition), propertyReference.PropertyType);
    }

    void InjectEqualityCheck(Instruction targetInstruction, TypeReference targetType)
    {

        var nopInstruction = instructions.First();
        if (nopInstruction.OpCode != OpCodes.Nop)
        {
            nopInstruction = Instruction.Create(OpCodes.Nop);
            instructions.Insert(0, nopInstruction);
        }

        var typeEqualityMethod = typeEqualityFinder.Find(targetType);
        if (typeEqualityMethod == null)
        {
            if (targetType.IsGenericParameter)
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Ceq),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
            else
            {
                if (targetType.SupportsCeq())
                {
                    instructions.Prepend(
                        Instruction.Create(OpCodes.Ldarg_1),
                        Instruction.Create(OpCodes.Ceq),
                        Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                        Instruction.Create(OpCodes.Ret));
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            instructions.Prepend(
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, typeEqualityMethod),
                Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                Instruction.Create(OpCodes.Ret));
        }
        instructions.Prepend(
            Instruction.Create(OpCodes.Ldarg_0),
            targetInstruction);
    }

}
