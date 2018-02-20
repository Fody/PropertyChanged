using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class EventArgsCache
{
    public EventArgsCache(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
        cacheTypeDefinition = new TypeDefinition(null, "<>PropertyChangedEventArgs", TypeAttributes.AutoClass | TypeAttributes.AutoLayout | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.NotPublic, moduleWeaver.ModuleDefinition.TypeSystem.Object);
    }

    public FieldReference GetEventArgsField(string propertyName)
    {
        if (!properties.TryGetValue(propertyName, out var field))
        {
            field = new FieldDefinition(propertyName, FieldAttributes.Assembly | FieldAttributes.Static | FieldAttributes.InitOnly, moduleWeaver.PropertyChangedEventArgsReference);
            properties.Add(propertyName, field);
        }

        return field;
    }

    public void InjectType()
    {
        if (properties.Count == 0)
            return;

        var cctor = new MethodDefinition(".cctor", MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static, moduleWeaver.ModuleDefinition.TypeSystem.Void);

        foreach (var pair in properties.OrderBy(i => i.Key))
        {
            var propertyName = pair.Key;
            var eventArgsField = pair.Value;

            cacheTypeDefinition.Fields.Add(eventArgsField);

            cctor.Body.Instructions.Append(
                Instruction.Create(OpCodes.Ldstr, propertyName),
                Instruction.Create(OpCodes.Newobj, moduleWeaver.PropertyChangedEventConstructorReference),
                Instruction.Create(OpCodes.Stsfld, eventArgsField)
            );
        }

        cctor.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ret)
        );

        cacheTypeDefinition.Methods.Add(cctor);
        moduleWeaver.ModuleDefinition.Types.Add(cacheTypeDefinition);
    }

    readonly ModuleWeaver moduleWeaver;
    TypeDefinition cacheTypeDefinition;
    Dictionary<string, FieldDefinition> properties = new Dictionary<string, FieldDefinition>();
}
