using Mono.Cecil;
using Mono.Cecil.Cil;

public class DelegateHolderInjector
{
    public TypeDefinition TargetTypeDefinition;
    public MethodReference OnPropertyChangedMethodReference;
    public ModuleWeaver ModuleWeaver;

    public void InjectDelegateHolder()
    {
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit, ModuleWeaver.ModuleDefinition.TypeSystem.Object);
        CreateFields(TargetTypeDefinition);
        CreateOnPropChanged(OnPropertyChangedMethodReference);
        CreateConstructor();
        TargetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        TargetField = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(TargetField);
        PropertyNameField = new FieldDefinition("propertyName", FieldAttributes.Public, ModuleWeaver.ModuleDefinition.TypeSystem.String);
        TypeDefinition.Fields.Add(PropertyNameField);
    }

    void CreateOnPropChanged(MethodReference onPropertyChangedMethodReference)
    {
        MethodDefinition = new MethodDefinition("OnPropertyChanged", MethodAttributes.Public | MethodAttributes.HideBySig, ModuleWeaver.ModuleDefinition.TypeSystem.Void);
        MethodDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, TargetField),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, PropertyNameField),
            Instruction.Create(OpCodes.Callvirt, onPropertyChangedMethodReference),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(MethodDefinition);
    }

    void CreateConstructor()
    {
        ConstructorDefinition = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, ModuleWeaver.ModuleDefinition.TypeSystem.Void);
        ConstructorDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Call, ModuleWeaver.ObjectConstructor),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(ConstructorDefinition);
    }

    public MethodDefinition MethodDefinition;
    public FieldDefinition PropertyNameField;
    public FieldDefinition TargetField;
    public TypeDefinition TypeDefinition;
    public MethodDefinition ConstructorDefinition;

}