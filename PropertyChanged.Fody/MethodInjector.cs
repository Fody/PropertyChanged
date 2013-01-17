using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    public EventInvokerMethod AddOnPropertyChangedMethod(TypeDefinition targetType)
    {
        var propertyChangedField = FindPropertyChangedField(targetType);
        if (propertyChangedField == null)
        {
            return null;
        }
        if (Found)
        {
            var methodDefinition = GetMethodDefinition(targetType, propertyChangedField);

            return new EventInvokerMethod
                       {
                           MethodReference = InjectInterceptedMethod(targetType, methodDefinition).GetGeneric(),
                           IsBeforeAfter = IsBeforeAfter,
                       };
        }
        return new EventInvokerMethod
                   {
                       MethodReference = InjectMethod(targetType, EventInvokerNames.First(), propertyChangedField).GetGeneric(),
                       IsBeforeAfter = false,
                   };
    }

    MethodDefinition GetMethodDefinition(TypeDefinition targetType, FieldReference propertyChangedField)
    {
        var eventInvokerName = "Inner" + EventInvokerNames.First();
        var methodDefinition = targetType.Methods.FirstOrDefault(x => x.Name == eventInvokerName);
        if (methodDefinition != null)
        {
            if (methodDefinition.Parameters.Count == 1 && methodDefinition.Parameters[0].ParameterType.FullName == "System.String")
            {
                return methodDefinition;
            }
        }
        return InjectMethod(targetType, eventInvokerName, propertyChangedField);
    }

    MethodDefinition InjectMethod(TypeDefinition targetType, string eventInvokerName, FieldReference propertyChangedField)
    {
        var method = new MethodDefinition(eventInvokerName, GetMethodAttributes(targetType), ModuleDefinition.TypeSystem.Void);
        method.Parameters.Add(new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.TypeSystem.String));

        var handlerVariable = new VariableDefinition(PropChangedHandlerReference);
        method.Body.Variables.Add(handlerVariable);
        var boolVariable = new VariableDefinition(ModuleDefinition.TypeSystem.Boolean);
        method.Body.Variables.Add(boolVariable);

        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, propertyChangedField)); 
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldnull));
        instructions.Add(Instruction.Create(OpCodes.Ceq));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Brtrue_S, last));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Newobj, ComponentModelPropertyChangedEventConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Callvirt, ComponentModelPropertyChangedEventHandlerInvokeReference));

        instructions.Add(last);
        method.Body.InitLocals = true;
        targetType.Methods.Add(method);
        return method;
    }

    static MethodAttributes GetMethodAttributes(TypeDefinition targetType)
    {
        if (targetType.IsSealed)
        {
            return MethodAttributes.Public | MethodAttributes.HideBySig;
        }
        return MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.NewSlot;
    }

    public static FieldReference FindPropertyChangedField(TypeDefinition targetType)
    {
        var findPropertyChangedField = targetType.Fields.FirstOrDefault(x => ModuleWeaver.IsPropertyChangedEventHandler(x.FieldType));
        if (findPropertyChangedField == null)
        {
            return null;
        }
        return findPropertyChangedField.GetGeneric();
    }

    MethodDefinition InjectInterceptedMethod(TypeDefinition targetType, MethodDefinition innerOnPropertyChanged)
    {
        InjectDelegateHolder(targetType, innerOnPropertyChanged);
        var method = new MethodDefinition(EventInvokerNames.First(), GetMethodAttributes(targetType), ModuleDefinition.TypeSystem.Void);

        var propertyName = new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.TypeSystem.String);
        method.Parameters.Add(propertyName);
        if (IsBeforeAfter)
        {
            var before = new ParameterDefinition("before", ParameterAttributes.None, ModuleDefinition.TypeSystem.Object);
            method.Parameters.Add(before);
            var after = new ParameterDefinition("after", ParameterAttributes.None, ModuleDefinition.TypeSystem.Object);
            method.Parameters.Add(after);
        }

        var action = new VariableDefinition("firePropertyChanged", ActionTypeReference);
        method.Body.Variables.Add(action);

        var variableDefinition = new VariableDefinition("delegateHolder", TypeDefinition);
        method.Body.Variables.Add(variableDefinition);


        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Newobj, ConstructorDefinition));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Stfld, PropertyName));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Stfld, Target));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldftn, MethodDefinition));
        instructions.Add(Instruction.Create(OpCodes.Newobj, ActionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, PropertyName));
        if (IsBeforeAfter)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
            instructions.Add(Instruction.Create(OpCodes.Call, InterceptMethod));
        }
        else
        {
            instructions.Add(Instruction.Create(OpCodes.Call, InterceptMethod));
        }

        instructions.Add(last);
        method.Body.InitLocals = true;

        targetType.Methods.Add(method);
        return method;
    }
}