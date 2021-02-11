using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using TypeSystem = Fody.TypeSystem;

public class PropertyWeaver
{
    ModuleWeaver moduleWeaver;
    PropertyData propertyData;
    TypeNode typeNode;
    TypeSystem typeSystem;
    MethodBody setMethodBody;
    Collection<Instruction> instructions;

    public PropertyWeaver(ModuleWeaver moduleWeaver, PropertyData propertyData, TypeNode typeNode, TypeSystem typeSystem)
    {
        this.moduleWeaver = moduleWeaver;
        this.propertyData = propertyData;
        this.typeNode = typeNode;
        this.typeSystem = typeSystem;
    }

    public void Execute()
    {
        moduleWeaver.WriteDebug("\t\t" + propertyData.PropertyDefinition.Name);
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
            return new List<int> { instructions.Count - 1 };
        }

        var setFieldInstructions = FindSetFieldInstructions().ToList();
        if (setFieldInstructions.Count == 0)
        {
            return new List<int> { instructions.Count - 1 };
        }

        return setFieldInstructions;
    }

    void InjectAtIndex(int index)
    {
        index = AddIsChangedSetterCall(index);

        foreach (var alsoNotifyForDefinition in propertyData.AlsoNotifyFor.Distinct())
        {
            var alsoNotifyMethods = GetMethodsForProperty(propertyData.ParentType, alsoNotifyForDefinition);
            
            index = AddEventInvokeCall(index, alsoNotifyMethods, alsoNotifyForDefinition);
        }

        var onChangedMethods = GetMethodsForProperty(propertyData.ParentType, propertyData.PropertyDefinition);
        AddEventInvokeCall(index, onChangedMethods, propertyData.PropertyDefinition);
    }

    List<OnChangedMethod> GetMethodsForProperty(TypeNode typeNode, PropertyDefinition property)
    {
        return (from m in typeNode.OnChangedMethods
            from p in m.Properties
            where p == property
            select m).ToList();
    }

    IEnumerable<int> FindSetFieldInstructions()
    {
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Stfld)
            {
                if (!(instruction.Operand is FieldReference fieldReference1))
                {
                    continue;
                }

                if (fieldReference1.Name == propertyData.BackingFieldReference.Name)
                {
                    yield return index + 1;
                }

                continue;
            }

            if (instruction.OpCode != OpCodes.Ldflda)
            {
                continue;
            }

            if (instruction.Next == null)
            {
                continue;
            }

            if (instruction.Next.OpCode != OpCodes.Initobj)
            {
                continue;
            }

            if (!(instruction.Operand is FieldReference fieldReference2))
            {
                continue;
            }

            if (fieldReference2.Name == propertyData.BackingFieldReference.Name)
            {
                yield return index + 2;
            }
        }
    }

    int AddIsChangedSetterCall(int index)
    {
        if (!moduleWeaver.EnableIsChangedProperty || typeNode.IsChangedInvoker == null ||
            propertyData.PropertyDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotSetChangedAttribute") ||
            propertyData.PropertyDefinition.Name == "IsChanged")
        {
            return index;
        }
        moduleWeaver.WriteDebug("\t\t\tSet IsChanged");
        return instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldc_I4, 1),
            CreateIsChangedInvoker());

    }

    int AddEventInvokeCall(int index, List<OnChangedMethod> onChangedMethods, PropertyDefinition property)
    {
        index = AddOnChangedMethodCalls(index, onChangedMethods, property);
        if (propertyData.AlreadyNotifies.Contains(property.Name))
        {
            moduleWeaver.WriteDebug($"\t\t\t{property.Name} skipped since call already exists");
            return index;
        }

        moduleWeaver.WriteDebug($"\t\t\t{property.Name}");
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.BeforeAfterGeneric)
        {
            return AddBeforeAfterGenericInvokerCall(index, property);
        }

        if (typeNode.EventInvoker.InvokerType == InvokerTypes.BeforeAfter)
        {
            return AddBeforeAfterInvokerCall(index, property);
        }

        if (typeNode.EventInvoker.InvokerType == InvokerTypes.PropertyChangedArg)
        {
            return AddPropertyChangedArgInvokerCall(index, property);
        }

        if (typeNode.EventInvoker.InvokerType == InvokerTypes.SenderPropertyChangedArg)
        {
            return AddSenderPropertyChangedArgInvokerCall(index, property);
        }

        return AddSimpleInvokerCall(index, property);
    }

    int AddOnChangedMethodCalls(int index, List<OnChangedMethod> onChangedMethods, PropertyDefinition propertyDefinition)
    {
        foreach (var onChangedMethod in onChangedMethods)
        {
            if (onChangedMethod.IsDefaultMethod)
            {
                if (!moduleWeaver.InjectOnPropertyNameChanged)
                {
                    continue;
                }

                if (ContainsCallToMethod(onChangedMethod.MethodReference.Name))
                {
                    continue;
                }
            }

            switch (onChangedMethod.OnChangedType)
            {
                case OnChangedTypes.NoArg:
                    index = AddSimpleOnChangedCall(index, onChangedMethod.MethodReference);
                    break;

                case OnChangedTypes.BeforeAfter:
                    index = AddBeforeAfterOnChangedCall(index, propertyDefinition, onChangedMethod.MethodReference);
                    break;
            }
        }

        return index;
    }

    bool ContainsCallToMethod(string onChangingMethodName)
    {
        return instructions.Select(x => x.Operand)
            .OfType<MethodReference>()
            .Any(x => x.Name == onChangingMethodName);
    }

    int AddSimpleInvokerCall(int index, PropertyDefinition property)
    {
        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldstr, property.Name));

        return instructions.Insert(index, CallEventInvoker(property).ToArray());
    }

    int AddPropertyChangedArgInvokerCall(int index, PropertyDefinition property)
    {
        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldsfld, moduleWeaver.EventArgsCache.GetEventArgsField(property.Name)));

        return instructions.Insert(index, CallEventInvoker(property).ToArray());
    }

    int AddSenderPropertyChangedArgInvokerCall(int index, PropertyDefinition property)
    {
        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldsfld, moduleWeaver.EventArgsCache.GetEventArgsField(property.Name)));

        return instructions.Insert(index, CallEventInvoker(property).ToArray());
    }

    int AddBeforeAfterGenericInvokerCall(int index, PropertyDefinition property)
    {
        var beforeVariable = new VariableDefinition(property.PropertyType);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(property.PropertyType);
        setMethodBody.Variables.Add(afterVariable);

        index = InsertVariableAssignmentFromCurrentValue(index, property, afterVariable);

        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldstr, property.Name),
            Instruction.Create(OpCodes.Ldloc, beforeVariable),
            Instruction.Create(OpCodes.Ldloc, afterVariable));

        index = instructions.Insert(index, CallEventInvoker(property).ToArray());

        return AddBeforeVariableAssignment(index, property, beforeVariable);
    }

    int AddBeforeAfterInvokerCall(int index, PropertyDefinition property)
    {
        var beforeVariable = new VariableDefinition(typeSystem.ObjectReference);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(typeSystem.ObjectReference);
        setMethodBody.Variables.Add(afterVariable);

        index = InsertVariableAssignmentFromCurrentValue(index, property, afterVariable);

        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldstr, property.Name),
            Instruction.Create(OpCodes.Ldloc, beforeVariable),
            Instruction.Create(OpCodes.Ldloc, afterVariable));

        index = instructions.Insert(index, CallEventInvoker(property).ToArray());

        return AddBeforeVariableAssignment(index, property, beforeVariable);
    }

    int AddSimpleOnChangedCall(int index, MethodReference methodReference)
    {
        return instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(methodReference));
    }

    int AddBeforeAfterOnChangedCall(int index, PropertyDefinition property, MethodReference methodReference)
    {
        var beforeVariable = new VariableDefinition(typeSystem.ObjectReference);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(typeSystem.ObjectReference);
        setMethodBody.Variables.Add(afterVariable);
        index = InsertVariableAssignmentFromCurrentValue(index, property, afterVariable);

        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldloc, beforeVariable),
            Instruction.Create(OpCodes.Ldloc, afterVariable),
            CreateCall(methodReference)
        );

        return AddBeforeVariableAssignment(index, property, beforeVariable);
    }

    int AddBeforeVariableAssignment(int index, PropertyDefinition property, VariableDefinition beforeVariable)
    {
        var getMethod = property.GetMethod.GetGeneric();

        instructions.Prepend(
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(getMethod),
            Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
            Instruction.Create(OpCodes.Stloc, beforeVariable));

        return index + 4;
    }

    int InsertVariableAssignmentFromCurrentValue(int index, PropertyDefinition property, VariableDefinition variable)
    {
        var getMethod = property.GetMethod.GetGeneric();

        instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(getMethod),
            Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
            Instruction.Create(OpCodes.Stloc, variable));

        return index + 4;
    }

    public IEnumerable<Instruction> CallEventInvoker(PropertyDefinition propertyDefinition)
    {
        var method = typeNode.EventInvoker.MethodReference;

        if (method.HasGenericParameters)
        {
            var genericMethod = new GenericInstanceMethod(method);
            genericMethod.GenericArguments.Add(propertyDefinition.PropertyType);
            method = genericMethod;
        }

        var instructionList = new List<Instruction> { Instruction.Create(OpCodes.Callvirt, method) };

        if (method.ReturnType.FullName != typeSystem.VoidReference.FullName)
        {
            instructionList.Add(Instruction.Create(OpCodes.Pop));
        }

        return instructionList;
    }

    public Instruction CreateIsChangedInvoker()
    {
        return Instruction.Create(OpCodes.Callvirt, typeNode.IsChangedInvoker);
    }

    public Instruction CreateCall(MethodReference methodReference)
    {
        return Instruction.Create(OpCodes.Callvirt, methodReference);
    }
}