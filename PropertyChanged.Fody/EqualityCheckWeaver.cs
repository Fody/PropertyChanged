using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class EqualityCheckWeaver
{
    PropertyData propertyData;
    TypeDefinition typeDefinition;
    ModuleWeaver typeEqualityFinder;
    Collection<Instruction> instructions;

    public EqualityCheckWeaver(PropertyData propertyData, TypeDefinition typeDefinition, ModuleWeaver typeEqualityFinder)
    {
        this.propertyData = propertyData;
        this.typeDefinition = typeDefinition;
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
        if (propertyData.BackingFieldReference.FieldType.FullName == propertyData.PropertyDefinition.PropertyType.FullName)
        {
            InjectEqualityCheck(Instruction.Create(OpCodes.Ldfld, fieldReference), fieldReference.FieldType);   
        }
    }


    void CheckAgainstProperty()
    {
        var propertyReference = propertyData.PropertyDefinition;
        var methodDefinition = propertyData.PropertyDefinition.GetMethod.GetGeneric();
        InjectEqualityCheck(Instruction.Create(OpCodes.Call, methodDefinition), propertyReference.PropertyType);
    }

    void InjectEqualityCheck(Instruction targetInstruction, TypeReference targetType)
    {
        if (ShouldSkipEqualityCheck())
        {
            return;
        }
        var nopInstruction = instructions.First();
        if (nopInstruction.OpCode != OpCodes.Nop)
        {
            nopInstruction = Instruction.Create(OpCodes.Nop);
            instructions.Insert(0, nopInstruction);
        }

        var comparerRef = typeEqualityFinder.GetEqualityComparer(targetType);

        instructions.Prepend(
            Instruction.Create(OpCodes.Call, comparerRef.DefaultRef),
            Instruction.Create(OpCodes.Ldarg_0),
            targetInstruction,
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, comparerRef.EqualsRef),
            Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
            Instruction.Create(OpCodes.Ret)
        );
    }

    bool ShouldSkipEqualityCheck()
    {
        if (!typeEqualityFinder.CheckForEquality)
        {
            return true;
        }

        var attribute = "PropertyChanged.DoNotCheckEqualityAttribute";

        return typeDefinition.GetAllCustomAttributes().ContainsAttribute(attribute)
               || propertyData.PropertyDefinition.CustomAttributes.ContainsAttribute(attribute);
    }

}
