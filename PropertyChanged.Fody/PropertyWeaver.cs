using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class PropertyWeaver
{
    ModuleWeaver moduleWeaver;
    PropertyData propertyData;
    TypeNode typeNode;
    TypeSystem typeSystem;
    MethodBody setMethodBody;
    Collection<Instruction> instructions;

    public PropertyWeaver(ModuleWeaver moduleWeaver, PropertyData propertyData, TypeNode typeNode, TypeSystem typeSystem )
    {
        this.moduleWeaver = moduleWeaver;
        this.propertyData = propertyData;
        this.typeNode = typeNode;
        this.typeSystem = typeSystem;
    }

    public void Execute()
    {
        moduleWeaver.LogInfo("\t\t" + propertyData.PropertyDefinition.Name);
        var property = propertyData.PropertyDefinition;
        setMethodBody = property.SetMethod.Body;
        instructions = property.SetMethod.Body.Instructions;


        var indexes = GetIndexes();
        indexes.Reverse();
        foreach (var index in indexes)
        {
            InjectAtIndex(index);
        }
    }

    List<int> GetIndexes()
    {
        if (propertyData.BackingFieldReference == null)
        {
            return new List<int> { instructions.Count -1};
        }
        var setFieldInstructions = FindSetFieldInstructions().ToList();
        if (setFieldInstructions.Count == 0)
        {
            return new List<int> {instructions.Count-1};
        }
        return setFieldInstructions;
    }

    void InjectAtIndex(int index)
    {
        index = AddIsChangedSetterCall(index);
        var propertyDefinitions = propertyData.AlsoNotifyFor.Distinct();

        index = propertyDefinitions.Aggregate(index, AddEventInvokeCall);
        AddEventInvokeCall(index, propertyData.PropertyDefinition);
    }

    IEnumerable<int> FindSetFieldInstructions()
    {
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Stfld)
            {
                var fieldReference = instruction.Operand as FieldReference;
                if (fieldReference == null)
                {
                    continue;
                }

                if (fieldReference.Name == propertyData.BackingFieldReference.Name)
                {
                    yield return index + 1;
                }
            }
            else if (instruction.OpCode == OpCodes.Ldflda)
            {
                if (instruction.Next==null)
                {
                    continue;
                }
                if (instruction.Next.OpCode!=OpCodes.Initobj)
                {
                    continue;
                }
                var fieldReference = instruction.Operand as FieldReference;
                if (fieldReference == null)
                {
                    continue;
                }

                if (fieldReference.Name == propertyData.BackingFieldReference.Name)
                {
                    yield return index + 2;
                }
            }
        }
    }

    int AddIsChangedSetterCall(int index)
    {
        if (typeNode.IsChangedInvoker != null &&
            !propertyData.PropertyDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotSetChangedAttribute") &&
            propertyData.PropertyDefinition.Name != "IsChanged")
        {
            moduleWeaver.LogInfo("\t\t\tSet IsChanged");
            return instructions.Insert(index,
                                       Instruction.Create(OpCodes.Ldarg_0),
                                       Instruction.Create(OpCodes.Ldc_I4, 1),
                                       CreateIsChangedInvoker());
        }
        return index;
    }

    int AddEventInvokeCall(int index, PropertyDefinition property)
    {
        index = AddOnChangedMethodCall(index, property);
        if (propertyData.AlreadyNotifies.Contains(property.Name))
        {
            moduleWeaver.LogInfo(string.Format("\t\t\t{0} skipped since call already exists", property.Name));
            return index;
        }

        moduleWeaver.LogInfo(string.Format("\t\t\t{0}", property.Name));
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.BeforeAfter)
        {
            return AddBeforeAfterInvokerCall(index, property);
        }
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.PropertyChangedArg)
        {
            return AddPropertyChangedArgInvokerCall(index, property);
        }
        return AddSimpleInvokerCall(index, property);
    }

    int AddOnChangedMethodCall(int index, PropertyDefinition property)
    {
        var onChangedMethodName = string.Format("On{0}Changed", property.Name);
        var onChangedMethod = typeNode
            .OnChangedMethods
            .FirstOrDefault(x => x.MethodReference.Name == onChangedMethodName);
        if (onChangedMethod != null)
        {
            if (onChangedMethod.OnChangedType == OnChangedTypes.NoArg)
            {
                return instructions.Insert(index,
                    Instruction.Create(OpCodes.Ldarg_0),
                    CreateCall(onChangedMethod));
            }
        }
        return index;
    }

    int AddSimpleInvokerCall(int index, PropertyDefinition property)
    {
        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   CallEventInvoker());
    }

    int AddPropertyChangedArgInvokerCall(int index, PropertyDefinition property)
    {
        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   Instruction.Create(OpCodes.Newobj, moduleWeaver.ComponentModelPropertyChangedEventConstructorReference),
                                   CallEventInvoker());
    }

    int AddBeforeAfterInvokerCall(int index, PropertyDefinition property)
    {
        var beforeVariable = new VariableDefinition(typeSystem.Object);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(typeSystem.Object);
        setMethodBody.Variables.Add(afterVariable);
        var getMethod = property.GetMethod.GetGeneric();

        index = instructions.Insert(index,
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    CreateCall(getMethod),
                                    Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
                                    Instruction.Create(OpCodes.Stloc, afterVariable),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Ldstr, property.Name),
                                    Instruction.Create(OpCodes.Ldloc, beforeVariable),
                                    Instruction.Create(OpCodes.Ldloc, afterVariable),
                                    CallEventInvoker()
            );

        instructions.Prepend(
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(getMethod),
            Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
            Instruction.Create(OpCodes.Stloc, beforeVariable));
        return index + 4;
    }

    public Instruction CallEventInvoker()
    {
        return Instruction.Create(OpCodes.Callvirt, typeNode.EventInvoker.MethodReference);
    }

    public Instruction CreateIsChangedInvoker()
    {
        return Instruction.Create(OpCodes.Callvirt, typeNode.IsChangedInvoker);
    }

    public Instruction CreateCall(MethodReference methodReference)
    {
        return Instruction.Create(OpCodes.Callvirt, methodReference);
    }

    public Instruction CreateCall(OnChangedMethod onChangedMethod)
    {
        throw new NotImplementedException();
    }
}