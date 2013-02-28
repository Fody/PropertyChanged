namespace PropertyChanged.Fody
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class InterfaceInjector
    {
        private readonly TypeReference notifyPropertyChangedTypeReference;
        private readonly TypeReference propertyChangedEventHandlerTypeReference;
        private readonly TypeReference voidTypeReference;
        private readonly MethodReference combineMethodRef;
        private readonly MethodReference removeMethodRef;

        public InterfaceInjector(ModuleDefinition moduleDefinition)
        {
            this.notifyPropertyChangedTypeReference = moduleDefinition.Import(typeof(INotifyPropertyChanged));
            this.propertyChangedEventHandlerTypeReference = moduleDefinition.Import(typeof(PropertyChangedEventHandler));
            this.voidTypeReference = moduleDefinition.Import(typeof(void));

            var type = typeof(Delegate);
            var combineMethodBase = type.GetMethod("Combine", new[] { type, type });
            var removeMethodBase = type.GetMethod("Remove");
            this.combineMethodRef = moduleDefinition.Import(combineMethodBase);
            this.removeMethodRef = moduleDefinition.Import(removeMethodBase);
        }

        public FieldReference TryInjectINotifyPropertyChangedInterface(TypeDefinition targetType)
        {
            if (ModuleWeaver.HasPropertyChangedEvent(targetType))
            {
                return null;
            }

            if (!AlreadyImplementsInterface(targetType))
            {
                targetType.Interfaces.Add(this.notifyPropertyChangedTypeReference);
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

            var eventField = new FieldDefinition("PropertyChanged", FieldAttributes.Private, this.propertyChangedEventHandlerTypeReference);
            type.Fields.Add(eventField);

            var eventDefinition = new EventDefinition(EventName, EventAttributes.None, this.propertyChangedEventHandlerTypeReference)
            {
                AddMethod = this.CreateEventMethod(string.Format("add_{0}", EventName), this.combineMethodRef, eventField),
                RemoveMethod = this.CreateEventMethod(string.Format("remove_{0}", EventName), this.removeMethodRef, eventField)
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

            var methodDef = new MethodDefinition(methodName, Attributes, this.voidTypeReference);

            methodDef.Parameters.Add(new ParameterDefinition(this.propertyChangedEventHandlerTypeReference));

            var cilWorker = methodDef.Body.GetILProcessor();
            cilWorker.Emit(OpCodes.Ldarg_0);
            cilWorker.Emit(OpCodes.Ldarg_0);
            cilWorker.Emit(OpCodes.Ldfld, eventField);
            cilWorker.Emit(OpCodes.Ldarg_1);
            cilWorker.Emit(OpCodes.Call, delegateMethodReference);
            cilWorker.Emit(OpCodes.Castclass, this.propertyChangedEventHandlerTypeReference);
            cilWorker.Emit(OpCodes.Stfld, eventField);
            cilWorker.Emit(OpCodes.Ret);
            return methodDef;
        }
    }
}