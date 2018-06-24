using Mono.Cecil;
using Mono.Cecil.Cil;

public class DelegateHolderInjector
{
    public TypeDefinition TargetTypeDefinition;
    public MethodReference OnPropertyChangedMethodReference;
    public ModuleWeaver ModuleWeaver;

    public void InjectDelegateHolder()
    {
        var attributes = TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit;
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", attributes, ModuleWeaver.TypeSystem.ObjectReference);
        CreateFields(TargetTypeDefinition);
        CreateOnPropChanged(OnPropertyChangedMethodReference);
        CreateConstructor();
        TargetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        TargetField = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(TargetField);
        PropertyNameField = new FieldDefinition("propertyName", FieldAttributes.Public, ModuleWeaver.TypeSystem.StringReference);
        TypeDefinition.Fields.Add(PropertyNameField);
    }

    void CreateOnPropChanged(MethodReference onPropertyChangedMethodReference)
    {
        var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        MethodDefinition = new MethodDefinition("OnPropertyChanged", attributes, ModuleWeaver.TypeSystem.VoidReference);
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
        var attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        ConstructorDefinition = new MethodDefinition(".ctor", attributes, ModuleWeaver.TypeSystem.VoidReference);
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