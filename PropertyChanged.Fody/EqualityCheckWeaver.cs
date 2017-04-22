using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
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
            typeEqualityFinder.LogDebug($"\t\t\tEquality Check Skipped for {targetType.Name}");
            return;
        }
        var nopInstruction = instructions.First();
        if (nopInstruction.OpCode != OpCodes.Nop)
        {
            nopInstruction = Instruction.Create(OpCodes.Nop);
            instructions.Insert(0, nopInstruction);
        }
        if (targetType.Name == "String")
        {
            instructions.Prepend(
                Instruction.Create(OpCodes.Ldarg_0),
                targetInstruction,
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldc_I4, typeEqualityFinder.OrdinalStringComparison),
                Instruction.Create(OpCodes.Call, typeEqualityFinder.StringEquals),
                Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                Instruction.Create(OpCodes.Ret));
            return;
        }
        var typeEqualityMethod = typeEqualityFinder.FindTypeEquality(targetType);
        if (typeEqualityMethod == null)
        {
            if (targetType.SupportsCeq() && targetType.IsValueType)
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ceq),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
            else if (targetType.IsValueType && typeEqualityFinder.EqualityComparerTypeReference != null)
            {
                var module = typeEqualityFinder.ModuleDefinition;
                var ec = typeEqualityFinder.EqualityComparerTypeReference.Resolve();

                var specificEqualityComparerType = module.ImportReference(ec.MakeGenericInstanceType(targetType));
                var defaultProperty = module.ImportReference(ec.Properties.Single(p => p.Name == "Default").GetMethod);
                var equalsMethod = module.ImportReference(ec.Methods.Single(p => p.Name == "Equals" && p.Parameters.Count == 2));

                defaultProperty.DeclaringType = specificEqualityComparerType;
                equalsMethod.DeclaringType = specificEqualityComparerType;

                instructions.Prepend(
                    Instruction.Create(OpCodes.Call, defaultProperty),
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Callvirt, equalsMethod),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
            else if (targetType.IsValueType || targetType.IsGenericParameter)
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Box, targetType),
                    Instruction.Create(OpCodes.Call, typeEqualityFinder.ObjectEqualsMethod),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
            else
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Call, typeEqualityFinder.ObjectEqualsMethod),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
        }
        else
        {
            instructions.Prepend(
                Instruction.Create(OpCodes.Ldarg_0),
                targetInstruction,
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, typeEqualityMethod),
                Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                Instruction.Create(OpCodes.Ret));
        }
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
