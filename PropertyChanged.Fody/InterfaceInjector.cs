using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

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

        return this.WeaveEvent(targetType);
    }

    private static bool AlreadyImplementsInterface(TypeDefinition targetType)
    {
        return targetType.Interfaces.Any(x => x.FullName == "System.ComponentModel.INotifyPropertyChanged");
    }

    // Thank you to Romain Verdier
    // largely copied from http://codingly.com/2008/11/10/introduction-a-monocecil-implementer-inotifypropertychanged/
    private FieldReference WeaveEvent(TypeDefinition type)
    {
        const string EventName = "PropertyChanged";

        var propertyChangedField = new FieldDefinition("PropertyChanged", FieldAttributes.Private, PropChangedHandlerReference);
        type.Fields.Add(propertyChangedField);

        var eventDefinition = new EventDefinition(EventName, EventAttributes.None, PropChangedHandlerReference)
        {
            AddMethod = this.CreateEventMethod(string.Format("add_{0}", EventName), this.DelegateCombineMethodRef, propertyChangedField),
            RemoveMethod = this.CreateEventMethod(string.Format("remove_{0}", EventName), this.DelegateRemoveMethodRef, propertyChangedField)
        };

        type.Methods.Add(eventDefinition.AddMethod);
        type.Methods.Add(eventDefinition.RemoveMethod);
        type.Events.Add(eventDefinition);

        return propertyChangedField;
    }
    
    private MethodDefinition CreateEventMethod(string methodName, MethodReference delegateMethodReference, FieldReference propertyChangedField)
    {
        const MethodAttributes Attributes = MethodAttributes.Public |
                                            MethodAttributes.HideBySig |
                                            MethodAttributes.Final |
                                            MethodAttributes.SpecialName |
                                            MethodAttributes.NewSlot |
                                            MethodAttributes.Virtual;

        var method = new MethodDefinition(methodName, Attributes, this.VoidTypeReference);

        method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, PropChangedHandlerReference));
        var handlerVariable = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable);
        method.Body.Variables.Add(handlerVariable);
        method.Body.Variables.Add(handlerVariable);
        var boolVariable = new VariableDefinition(ModuleDefinition.TypeSystem.Boolean);
        method.Body.Variables.Add(boolVariable);

        Collection<Instruction> instructions = method.Body.Instructions;

        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, propertyChangedField)); 
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));

        var loopBegin = Instruction.Create(OpCodes.Ldloc_0);
        instructions.Add(loopBegin);
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Call, delegateMethodReference));
        instructions.Add(Instruction.Create(OpCodes.Castclass, PropChangedHandlerReference));
        instructions.Add(Instruction.Create(OpCodes.Stloc_2));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldflda, propertyChangedField));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_2));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Call, InterlockedCompareExchangeForPropChangedHandler));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ceq));
        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        instructions.Add(Instruction.Create(OpCodes.Ceq));
        instructions.Add(Instruction.Create(OpCodes.Stloc_3));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_3));
        instructions.Add(Instruction.Create(OpCodes.Brtrue_S, loopBegin)); // goto begin of loop

        instructions.Add(Instruction.Create(OpCodes.Ret));
        method.Body.InitLocals = true;

        return method;
    }
}