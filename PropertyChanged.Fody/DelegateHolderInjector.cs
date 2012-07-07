using Mono.Cecil;
using Mono.Cecil.Cil;

public class DelegateHolderInjector
{
    MsCoreReferenceFinder msCoreReferenceFinder;
    TypeSystem typeSystem;

    public DelegateHolderInjector(MsCoreReferenceFinder msCoreReferenceFinder, TypeSystem typeSystem)
    {
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.typeSystem = typeSystem;
    }

    public void Execute(TypeDefinition targetTypeDefinition, MethodReference onPropertyChangedMethodReference)
    {
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit, typeSystem.Object);
        CreateFields(targetTypeDefinition);
        CreateOnPropChanged(onPropertyChangedMethodReference);
        CreateConstructor();
        targetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        Target = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(Target);
        PropertyName = new FieldDefinition("propertyName", FieldAttributes.Public, typeSystem.String);
        TypeDefinition.Fields.Add(PropertyName);
    }

    void CreateOnPropChanged(MethodReference onPropertyChangedMethodReference)
    {
        MethodDefinition = new MethodDefinition("OnPropertyChanged", MethodAttributes.Public | MethodAttributes.HideBySig, typeSystem.Void);
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
        ConstructorDefinition = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeSystem.Void);
        ConstructorDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Call, msCoreReferenceFinder.ObjectConstructor),
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