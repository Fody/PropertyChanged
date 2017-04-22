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

        if (FoundInterceptor)
        {
            if (targetType.HasGenericParameters)
            {
                var message = $"Error processing '{targetType.Name}'. Interception is not supported on generic types. To manually work around this problem add a [DoNotNotify] to the class and then manually implement INotifyPropertyChanged for that class and all child classes. If you would like this feature handled automatically please feel free to submit a pull request.";
                throw new WeavingException(message);
            }
            var methodDefinition = GetMethodDefinition(targetType, propertyChangedField);

            return new EventInvokerMethod
                       {
                           MethodReference = InjectInterceptedMethod(targetType, methodDefinition).GetGeneric(),
                           InvokerType = InterceptorType,
                           IsVisibleFromChildren = true,
                       };
        }
        return new EventInvokerMethod
                   {
                       MethodReference = InjectMethod(targetType, EventInvokerNames.First(), propertyChangedField).GetGeneric(),
                       InvokerType = InterceptorType,
                       IsVisibleFromChildren = true,
                   };
    }

    MethodDefinition GetMethodDefinition(TypeDefinition targetType, FieldReference propertyChangedField)
    {
        var eventInvokerName = "Inner" + EventInvokerNames.First();
        var methodDefinition = targetType.Methods.FirstOrDefault(x => x.Name == eventInvokerName);
        if (methodDefinition?.Parameters.Count == 1 && methodDefinition.Parameters[0].ParameterType.FullName == "System.String")
        {
            return methodDefinition;
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
        var findPropertyChangedField = targetType.Fields.FirstOrDefault(x => IsPropertyChangedEventHandler(x.FieldType));
        return findPropertyChangedField?.GetGeneric();
    }

    MethodDefinition InjectInterceptedMethod(TypeDefinition targetType, MethodDefinition innerOnPropertyChanged)
    {
        var delegateHolderInjector = new DelegateHolderInjector
                                     {
                                         TargetTypeDefinition = targetType,
                                         OnPropertyChangedMethodReference = innerOnPropertyChanged,
                                         ModuleWeaver = this,
                                     };
        delegateHolderInjector.InjectDelegateHolder();
        var method = new MethodDefinition(EventInvokerNames.First(), GetMethodAttributes(targetType), ModuleDefinition.TypeSystem.Void);

        var propertyName = new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.TypeSystem.String);
        method.Parameters.Add(propertyName);
        if (InterceptorType == InvokerTypes.BeforeAfter)
        {
            var before = new ParameterDefinition("before", ParameterAttributes.None, ModuleDefinition.TypeSystem.Object);
            method.Parameters.Add(before);
            var after = new ParameterDefinition("after", ParameterAttributes.None, ModuleDefinition.TypeSystem.Object);
            method.Parameters.Add(after);
        }

        var action = new VariableDefinition(ActionTypeReference);
        method.Body.Variables.Add(action);

        var variableDefinition = new VariableDefinition(delegateHolderInjector.TypeDefinition);
        method.Body.Variables.Add(variableDefinition);


        var instructions = method.Body.Instructions;

        var last = Instruction.Create(OpCodes.Ret);
        instructions.Add(Instruction.Create(OpCodes.Newobj, delegateHolderInjector.ConstructorDefinition));
        instructions.Add(Instruction.Create(OpCodes.Stloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Stfld, delegateHolderInjector.PropertyNameField));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Stfld, delegateHolderInjector.TargetField));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldftn, delegateHolderInjector.MethodDefinition));
        instructions.Add(Instruction.Create(OpCodes.Newobj, ActionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
        instructions.Add(Instruction.Create(OpCodes.Ldloc_1));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, delegateHolderInjector.PropertyNameField));
        if (InterceptorType == InvokerTypes.BeforeAfter)
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