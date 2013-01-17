using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    public void InjectDelegateHolder(TypeDefinition targetTypeDefinition, MethodReference onPropertyChangedMethodReference)
    {
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit, ModuleDefinition.TypeSystem.Object);
        CreateFields(targetTypeDefinition);
        CreateOnPropChanged(onPropertyChangedMethodReference);
        CreateConstructor();
        targetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        Target = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(Target);
        PropertyName = new FieldDefinition("propertyName", FieldAttributes.Public, ModuleDefinition.TypeSystem.String);
        TypeDefinition.Fields.Add(PropertyName);
    }

    void CreateOnPropChanged(MethodReference onPropertyChangedMethodReference)
    {
        MethodDefinition = new MethodDefinition("OnPropertyChanged", MethodAttributes.Public | MethodAttributes.HideBySig, ModuleDefinition.TypeSystem.Void);
        MethodDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, Target),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, PropertyName),
            Instruction.Create(OpCodes.Callvirt, onPropertyChangedMethodReference),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(MethodDefinition);
    }

    void CreateConstructor()
    {
        ConstructorDefinition = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, ModuleDefinition.TypeSystem.Void);
        ConstructorDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Call, ObjectConstructor),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(ConstructorDefinition);
    }

    public MethodDefinition MethodDefinition;
    public FieldDefinition PropertyName;
    public FieldDefinition Target;
    public TypeDefinition TypeDefinition;
    public MethodDefinition ConstructorDefinition;
}