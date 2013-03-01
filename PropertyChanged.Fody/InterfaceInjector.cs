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

    private static bool AlreadyImplementsInterface(TypeDefinition targetType)
    {
        return targetType.Interfaces.Any(x => x.FullName == "System.ComponentModel.INotifyPropertyChanged");
    }

    // Thank you to Romain Verdier
    // largely copied from http://codingly.com/2008/11/10/introduction-a-monocecil-implementer-inotifypropertychanged/
    private FieldReference WeaveEvent(TypeDefinition type)
    {
        const string EventName = "PropertyChanged";

        var eventField = new FieldDefinition("PropertyChanged", FieldAttributes.Private, PropChangedHandlerReference);
        type.Fields.Add(eventField);

        var eventDefinition = new EventDefinition(EventName, EventAttributes.None, PropChangedHandlerReference)
        {
            AddMethod = CreateEventMethod(string.Format("add_{0}", EventName), this.DelegateCombineMethodRef, eventField),
            RemoveMethod = CreateEventMethod(string.Format("remove_{0}", EventName), this.DelegateRemoveMethodRef, eventField)
        };

        type.Methods.Add(eventDefinition.AddMethod);
        type.Methods.Add(eventDefinition.RemoveMethod);
        type.Events.Add(eventDefinition);

        return eventField;
    }

    private MethodDefinition CreateEventMethod(string methodName, MethodReference delegateMethodReference, FieldReference eventField)
    {
        const MethodAttributes Attributes = MethodAttributes.Public |
                                            MethodAttributes.HideBySig |
                                            MethodAttributes.Final |
                                            MethodAttributes.SpecialName |
                                            MethodAttributes.NewSlot |
                                            MethodAttributes.Virtual;

        var methodDef = new MethodDefinition(methodName, Attributes, this.VoidTypeReference);

        methodDef.Parameters.Add(new ParameterDefinition(PropChangedHandlerReference));

        var cilWorker = methodDef.Body.GetILProcessor();
        cilWorker.Emit(OpCodes.Ldarg_0);
        cilWorker.Emit(OpCodes.Ldarg_0);
        cilWorker.Emit(OpCodes.Ldfld, eventField);
        cilWorker.Emit(OpCodes.Ldarg_1);
        cilWorker.Emit(OpCodes.Call, delegateMethodReference);
        cilWorker.Emit(OpCodes.Castclass, PropChangedHandlerReference);
        cilWorker.Emit(OpCodes.Stfld, eventField);
        cilWorker.Emit(OpCodes.Ret);
        return methodDef;
    }
}