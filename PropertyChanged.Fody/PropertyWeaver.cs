using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using TypeSystem = Fody.TypeSystem;

public class PropertyWeaver(
    ModuleWeaver weaver,
    PropertyData propertyData,
    TypeNode typeNode,
    TypeSystem typeSystem)
{
    MethodBody setMethodBody;
    Collection<Instruction> instructions;

    public void Execute()
    {
        weaver.WriteDebug("\t\t" + propertyData.PropertyDefinition.Name);
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
            return [instructions.Count - 1];
        }

        var setFieldInstructions = FindSetFieldInstructions().ToList();
        if (setFieldInstructions.Count == 0)
        {
            return [instructions.Count - 1];
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

    static List<OnChangedMethod> GetMethodsForProperty(TypeNode typeNode, PropertyDefinition property)
    {
        return (from method in typeNode.OnChangedMethods
                from prop in method.Properties
                where prop == property
                select method).ToList();
    }

    IEnumerable<int> FindSetFieldInstructions()
    {
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Stfld)
            {
                if (instruction.Operand is not FieldReference fieldReference1)
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

            if (instruction.Operand is not FieldReference fieldReference2)
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
        if (!weaver.EnableIsChangedProperty || typeNode.IsChangedInvoker == null ||
            propertyData.PropertyDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotSetChangedAttribute") ||
            propertyData.PropertyDefinition.Name == "IsChanged")
        {
            return index;
        }
        weaver.WriteDebug("\t\t\tSet IsChanged");
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
            weaver.WriteDebug($"\t\t\t{property.Name} skipped since call already exists");
            return index;
        }

        weaver.WriteDebug($"\t\t\t{property.Name}");
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
                if (!weaver.InjectOnPropertyNameChanged)
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

                case OnChangedTypes.BeforeAfterTyped:
                    if (propertyDefinition.PropertyType.FullName != onChangedMethod.ArgumentTypeFullName)
                    {
                        var methodDefinition = onChangedMethod.MethodDefinition;
                        weaver.EmitConditionalWarning(methodDefinition, $"Unsupported signature for a On_PropertyName_Changed method: {methodDefinition.Name} in {methodDefinition.DeclaringType.FullName}");
                        break;
                    }
                    index = AddBeforeAfterOnChangedCall(index, propertyDefinition, onChangedMethod.MethodReference, true);
                    break;
            }
        }

        return index;
    }

    bool ContainsCallToMethod(string onChangingMethodName)
    {
        return instructions.Select(_ => _.Operand)
            .OfType<MethodReference>()
            .Any(_ => _.Name == onChangingMethodName);
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
            Instruction.Create(OpCodes.Ldsfld, weaver.EventArgsCache.GetEventArgsField(property.Name)));

        return instructions.Insert(index, CallEventInvoker(property).ToArray());
    }

    int AddSenderPropertyChangedArgInvokerCall(int index, PropertyDefinition property)
    {
        index = instructions.Insert(index,
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldsfld, weaver.EventArgsCache.GetEventArgsField(property.Name)));

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

    int AddBeforeAfterOnChangedCall(int index, PropertyDefinition property, MethodReference methodReference, bool useTypedParameters = false)
    {
        var variableType = useTypedParameters ? property.PropertyType : typeSystem.ObjectReference;

        var beforeVariable = new VariableDefinition(variableType);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(variableType);
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

    IEnumerable<Instruction> BuildVariableAssignmentInstructions(PropertyDefinition property, VariableDefinition variable)
    {
        var getMethod = property.GetMethod.GetGeneric();

        yield return Instruction.Create(OpCodes.Ldarg_0);
        yield return CreateCall(getMethod);

        var returnType = property.GetMethod.ReturnType;
        if (returnType.FullName != variable.VariableType.FullName)
        {
            yield return Instruction.Create(OpCodes.Box, returnType);
        }
        yield return Instruction.Create(OpCodes.Stloc, variable);
    }

    int AddBeforeVariableAssignment(int index, PropertyDefinition property, VariableDefinition variable)
    {
        var i = BuildVariableAssignmentInstructions(property, variable).ToArray();
        instructions.Prepend(i);
        return index + i.Length;
    }

    int InsertVariableAssignmentFromCurrentValue(int index, PropertyDefinition property, VariableDefinition variable)
    {
        var i = BuildVariableAssignmentInstructions(property, variable).ToArray();
        instructions.Insert(index, i);
        return index + i.Length;
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

        var instructionList = new List<Instruction>
        {
            Instruction.Create(typeNode.TypeDefinition.GetCallOpCode(), method)
        };

        if (method.ReturnType.FullName != typeSystem.VoidReference.FullName)
        {
            instructionList.Add(Instruction.Create(OpCodes.Pop));
        }

        return instructionList;
    }

    public Instruction CreateIsChangedInvoker()
    {
        return Instruction.Create(typeNode.TypeDefinition.GetCallOpCode(), typeNode.IsChangedInvoker);
    }

    public Instruction CreateCall(MethodReference methodReference)
    {
        return Instruction.Create(typeNode.TypeDefinition.GetCallOpCode(), methodReference);
    }
}