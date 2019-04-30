using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class EventArgsCache
{
    public EventArgsCache(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
        var attributes = TypeAttributes.AutoClass | TypeAttributes.AutoLayout | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.NotPublic;
        cacheTypeDefinition = new TypeDefinition(moduleWeaver.ModuleDefinition.Name, "<>PropertyChangedEventArgs", attributes, moduleWeaver.TypeSystem.ObjectReference);
    }

    public FieldReference GetEventArgsField(string propertyName)
    {
        if (!properties.TryGetValue(propertyName, out var field))
        {
            var attributes = FieldAttributes.Assembly | FieldAttributes.Static | FieldAttributes.InitOnly;
            field = new FieldDefinition(propertyName, attributes, moduleWeaver.PropertyChangedEventArgsReference);
            properties.Add(propertyName, field);
        }

        return field;
    }

    public void InjectType()
    {
        if (properties.Count == 0)
            return;

        var attributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
        var cctor = new MethodDefinition(".cctor", attributes, moduleWeaver.TypeSystem.VoidReference);

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

    ModuleWeaver moduleWeaver;
    TypeDefinition cacheTypeDefinition;
    Dictionary<string, FieldDefinition> properties = new Dictionary<string, FieldDefinition>();
}
