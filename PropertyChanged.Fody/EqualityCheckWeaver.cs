using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Mono.Cecil.Rocks;

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
            if (targetType.IsGenericParameter)
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
            else if (targetType.SupportsCeq())
            {
                instructions.Prepend(
                    Instruction.Create(OpCodes.Ldarg_0),
                    targetInstruction,
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ceq),
                    Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                    Instruction.Create(OpCodes.Ret));
            }
        }
        else
        {
            var propertyType = propertyData.PropertyDefinition.PropertyType;
            if (propertyType.IsGenericInstance && !propertyType.FullName.StartsWith("System.Nullable"))
            {
                // according to https://groups.google.com/forum/#!topic/mono-cecil/mCat5UuR47I
                // Resolve() looses generic arguments and MakeHostInstanceGeneric regenerates those
                // also no check for whether TypeReference is really GenericInstanceType (can't be otherwise, can be?)
                var git = (GenericInstanceType)propertyData.PropertyDefinition.PropertyType;
                instructions.Prepend(
                   Instruction.Create(OpCodes.Ldarg_0),
                   targetInstruction,
                   Instruction.Create(OpCodes.Ldarg_1),
                   Instruction.Create(OpCodes.Call, MakeHostInstanceGeneric(typeEqualityMethod, git.GenericArguments.ToArray())),
                   Instruction.Create(OpCodes.Brfalse_S, nopInstruction),
                   Instruction.Create(OpCodes.Ret));
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
    }

    /// <summary>
    /// Recreates generic arguments.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    /// <remarks>Taken from https://groups.google.com/forum/#!topic/mono-cecil/mCat5UuR47I</remarks>
    public static MethodReference MakeHostInstanceGeneric(MethodReference self, params TypeReference[] arguments)
    {
        var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
        {
            HasThis = self.HasThis,
            ExplicitThis = self.ExplicitThis,
            CallingConvention = self.CallingConvention
        };

        foreach (var parameter in self.Parameters)
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

        foreach (var generic_parameter in self.GenericParameters)
            reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

        return reference;
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
