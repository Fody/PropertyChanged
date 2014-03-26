using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    public void InjectINotifyPropertyChangedInterface(TypeDefinition targetType)
    {
        targetType.Interfaces.Add(PropChangedInterfaceReference);
        WeaveEvent(targetType);
    }

    void WeaveEvent(TypeDefinition type)
    {
        var propertyChangedFieldDef = new FieldDefinition("PropertyChanged", FieldAttributes.Private | FieldAttributes.NotSerialized, PropChangedHandlerReference);
        type.Fields.Add(propertyChangedFieldDef);
        var propertyChangedField = propertyChangedFieldDef.GetGeneric();

        var eventDefinition = new EventDefinition("PropertyChanged", EventAttributes.None, PropChangedHandlerReference)
            {
                AddMethod = CreateEventMethod("add_PropertyChanged", DelegateCombineMethodRef, propertyChangedField),
                RemoveMethod = CreateEventMethod("remove_PropertyChanged", DelegateRemoveMethodRef, propertyChangedField)
            };

        type.Methods.Add(eventDefinition.AddMethod);
        type.Methods.Add(eventDefinition.RemoveMethod);
        type.Events.Add(eventDefinition);
    }

    MethodDefinition CreateEventMethod(string methodName, MethodReference delegateMethodReference, FieldReference propertyChangedField)
    {
        const MethodAttributes Attributes = MethodAttributes.Public |
                                            MethodAttributes.HideBySig |
                                            MethodAttributes.Final |
                                            MethodAttributes.SpecialName |
                                            MethodAttributes.NewSlot |
                                            MethodAttributes.Virtual;

        var method = new MethodDefinition(methodName, Attributes, ModuleDefinition.TypeSystem.Void);

        method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, PropChangedHandlerReference));
        var handlerVariable0 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable0);
        var handlerVariable1 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable1);
        var handlerVariable2 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable2);

        var loopBegin = Instruction.Create(OpCodes.Ldloc, handlerVariable0);
        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, propertyChangedField),
            Instruction.Create(OpCodes.Stloc, handlerVariable0),
            loopBegin,
            Instruction.Create(OpCodes.Stloc, handlerVariable1),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Call, delegateMethodReference),
            Instruction.Create(OpCodes.Castclass, PropChangedHandlerReference),
            Instruction.Create(OpCodes.Stloc, handlerVariable2),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldflda, propertyChangedField),
            Instruction.Create(OpCodes.Ldloc, handlerVariable2),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Call, InterlockedCompareExchangeForPropChangedHandler),
            Instruction.Create(OpCodes.Stloc, handlerVariable0),
            Instruction.Create(OpCodes.Ldloc, handlerVariable0),
            Instruction.Create(OpCodes.Ldloc, handlerVariable1),
            Instruction.Create(OpCodes.Bne_Un_S, loopBegin), // goto begin of loop
            Instruction.Create(OpCodes.Ret));
        method.Body.InitLocals = true;
        method.Body.OptimizeMacros();

        return method;
    }
}