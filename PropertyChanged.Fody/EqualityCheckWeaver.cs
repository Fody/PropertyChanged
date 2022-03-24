using System;
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
    VariableDefinition resultEquals;

    public EqualityCheckWeaver(PropertyData propertyData, TypeDefinition typeDefinition, ModuleWeaver typeEqualityFinder)
    {
        this.propertyData = propertyData;
        this.typeDefinition = typeDefinition;
        this.typeEqualityFinder = typeEqualityFinder;
    }
    public EqualityCheckWeaver(PropertyData propertyData, TypeDefinition typeDefinition, ModuleWeaver typeEqualityFinder, VariableDefinition variableDefinition)
        : this(propertyData, typeDefinition, typeEqualityFinder)
    {
        resultEquals = variableDefinition;
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
            InjectEqualityCheck(Instruction.Create(OpCodes.Ldfld, fieldReference), fieldReference.FieldType, fieldReference.DeclaringType);
        }
    }

    void CheckAgainstProperty()
    {
        var propertyReference = propertyData.PropertyDefinition;
        var methodDefinition = propertyData.PropertyDefinition.GetMethod.GetGeneric();
        InjectEqualityCheck(Instruction.Create(OpCodes.Call, methodDefinition), propertyReference.PropertyType, propertyReference.DeclaringType);
    }

    void InjectEqualityCheck(Instruction targetInstruction, TypeReference targetType, TypeReference declaringType)
    {
        if (ShouldSkipEqualityCheck())
        {
            typeEqualityFinder.WriteDebug($"\t\t\tEquality Check Skipped for {targetType.Name}");
            return;
        }

        var nopInstruction = instructions.First();
        if (targetType.Name == "String")
        {
            instructions.Prepend(
                Instruction.Create(OpCodes.Ldarg_0),
                targetInstruction,
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldc_I4, typeEqualityFinder.OrdinalStringComparison),
                Instruction.Create(OpCodes.Call, typeEqualityFinder.StringEquals),
                Instruction.Create(OpCodes.Stloc, resultEquals));
            return;
        }
        var typeEqualityMethod = propertyData.EqualsMethod;
        if (typeEqualityMethod == null)
        {
            var supportsCeq = false;

            try
            {
                supportsCeq = targetType.SupportsCeq();
            }
            catch (Exception ex)
            {
                typeEqualityFinder.EmitWarning($"Ignoring Ceq of type {targetType.FullName} => {ex.Message}");
            }

            if (supportsCeq && (targetType.IsValueType || !typeEqualityFinder.CheckForEqualityUsingBaseEquals))
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ceq),
                    Instruction.Create(OpCodes.Stloc, resultEquals));
            }
            else if (targetType.IsValueType && typeEqualityFinder.EqualityComparerTypeReference != null)
            {
                var module = typeEqualityFinder.ModuleDefinition;
                var ec = typeEqualityFinder.EqualityComparerTypeReference.Resolve();

                var specificEqualityComparerType = module.ImportReference(ec.MakeGenericInstanceType(targetType), declaringType);
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
                    Instruction.Create(OpCodes.Stloc, resultEquals));
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
                    Instruction.Create(OpCodes.Stloc, resultEquals));
            }
            else
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Call, typeEqualityFinder.ObjectEqualsMethod),
                    Instruction.Create(OpCodes.Stloc, resultEquals));
            }
        }
        else
        {
            instructions.Prepend(
                Instruction.Create(OpCodes.Ldarg_0),
                targetInstruction,
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, typeEqualityMethod),
                Instruction.Create(OpCodes.Stloc, resultEquals));
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
