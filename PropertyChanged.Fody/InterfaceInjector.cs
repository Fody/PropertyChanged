using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public FieldReference TryInjectINotifyPropertyChangedInterface(TypeDefinition targetType)
    {
        if (HasPropertyChangedEvent(targetType))
        {
            return null;
        }

        if (!AlreadyImplementsInterface(targetType))
        {
            targetType.Interfaces.Add(PropChangedInterfaceReference);
        }

        return WeaveEvent(targetType);
    }

    static bool AlreadyImplementsInterface(TypeDefinition targetType)
    {
        return targetType.Interfaces.Any(x => x.FullName == "System.ComponentModel.INotifyPropertyChanged");
    }

    // Thank you to Romain Verdier
    // largely copied from http://codingly.com/2008/11/10/introduction-a-monocecil-implementer-inotifypropertychanged/
    FieldReference WeaveEvent(TypeDefinition type)
    {
        var propertyChangedField = new FieldDefinition("PropertyChanged", FieldAttributes.Private, PropChangedHandlerReference);
        type.Fields.Add(propertyChangedField);

        var eventDefinition = new EventDefinition("PropertyChanged", EventAttributes.None, PropChangedHandlerReference)
            {
                AddMethod = CreateEventMethod("add_PropertyChanged", DelegateCombineMethodRef, propertyChangedField),
                RemoveMethod = CreateEventMethod("remove_PropertyChanged", DelegateRemoveMethodRef, propertyChangedField)
            };

        type.Methods.Add(eventDefinition.AddMethod);
        type.Methods.Add(eventDefinition.RemoveMethod);
        type.Events.Add(eventDefinition);

        return propertyChangedField;
    }

    MethodDefinition CreateEventMethod(string methodName, MethodReference delegateMethodReference, FieldReference propertyChangedField)
    {
        const MethodAttributes Attributes = MethodAttributes.Public |
                                            MethodAttributes.HideBySig |
                                            MethodAttributes.Final |
                                            MethodAttributes.SpecialName |
                                            MethodAttributes.NewSlot |
                                            MethodAttributes.Virtual;

        var method = new MethodDefinition(methodName, Attributes, VoidTypeReference);

        method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, PropChangedHandlerReference));
        var handlerVariable0 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable0);
        var handlerVariable1 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable1);
        var handlerVariable2 = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable2);
        var boolVariable = new VariableDefinition(ModuleDefinition.TypeSystem.Boolean);
        method.Body.Variables.Add(boolVariable);

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
            Instruction.Create(OpCodes.Ceq),
            Instruction.Create(OpCodes.Ldc_I4_0),
            Instruction.Create(OpCodes.Ceq),
            Instruction.Create(OpCodes.Stloc, boolVariable),
            Instruction.Create(OpCodes.Ldloc, boolVariable),
            Instruction.Create(OpCodes.Brtrue_S, loopBegin), // goto begin of loop
            Instruction.Create(OpCodes.Ret));
        method.Body.InitLocals = true;

        return method;
    }
}