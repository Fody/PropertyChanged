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
        moduleWeaver.LogDebug("\t\t" + propertyData.PropertyDefinition.Name);
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
                if (!(instruction.Operand is FieldReference fieldReference))
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
                if (instruction.Next == null)
                {
                    continue;
                }
                if (instruction.Next.OpCode != OpCodes.Initobj)
                {
                    continue;
                }

                if (!(instruction.Operand is FieldReference fieldReference))
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
            moduleWeaver.LogDebug("\t\t\tSet IsChanged");
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
            moduleWeaver.LogDebug($"\t\t\t{property.Name} skipped since call already exists");
            return index;
        }

        moduleWeaver.LogDebug($"\t\t\t{property.Name}");
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.BeforeAfterGenericMethod)
        {
            return AddBeforeAfterGenericInvokerCall(index, property);
        }
        if (typeNode.EventInvoker.InvokerType == InvokerTypes.BeforeAfterGenericParameters)
        {
            return AddBeforeAfterInvokerCallWithGenericParameters(index, property);
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

    int AddOnChangedMethodCall(int index, PropertyDefinition property)
    {
        if (!moduleWeaver.InjectOnPropertyNameChanged)
        {
            return index;
        }
        var onChangedMethodName = $"On{property.Name}Changed";
        if (ContainsCallToMethod(onChangedMethodName))
        {
            return index;
        }
        var onChangedMethod = typeNode
            .OnChangedMethods
            .FirstOrDefault(x => x.MethodReference.Name == onChangedMethodName);
        if (onChangedMethod == null)
        {
            return index;
        }

        if (onChangedMethod.OnChangedType == OnChangedTypes.NoArg)
        {
            return AddSimpleOnChangedCall(index, onChangedMethod.MethodReference);
        }

        if (onChangedMethod.OnChangedType == OnChangedTypes.BeforeAfterObject)
        {
            return AddBeforeAfterOnChangedCall(index, property, onChangedMethod.MethodReference, typeSystem.ObjectReference);
        }

        if (onChangedMethod.OnChangedType == OnChangedTypes.BeforeAfterType)
        {
            var methodParameterType = onChangedMethod.MethodReference.Parameters[0].ParameterType;
            if (methodParameterType == property.PropertyType)
            {
#if NETFRAMEWORK
                if (property.PropertyType.IsGenericParameter)
                {
                    return index;
                }
#endif
                return AddBeforeAfterOnChangedCall(index, property, onChangedMethod.MethodReference, property.PropertyType);
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

    int AddBeforeAfterInvokerCallWithGenericParameters(int index, PropertyDefinition property)
    {
#if NETFRAMEWORK
        return index;
#else
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
#endif
    }

    int AddSimpleOnChangedCall(int index, MethodReference methodReference)
    {
        return instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(methodReference));
    }

    int AddBeforeAfterOnChangedCall(int index, PropertyDefinition property, MethodReference methodReference, TypeReference typeReference)
    {
        var beforeVariable = new VariableDefinition(typeReference);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(typeReference);
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