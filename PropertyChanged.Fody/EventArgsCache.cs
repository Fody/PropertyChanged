using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class EventArgsCache
{
    public EventArgsCache(ModuleWeaver weaver)
    {
        this.weaver = weaver;
        var attributes = TypeAttributes.AutoClass | TypeAttributes.AutoLayout | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.NotPublic;
        cacheTypeDefinition = new(weaver.ModuleDefinition.Assembly.Name.Name, "<>PropertyChangedEventArgs", attributes, weaver.TypeSystem.ObjectReference);
        weaver.MarkAsGeneratedCode(cacheTypeDefinition.CustomAttributes);
    }

    public FieldReference GetEventArgsField(string propertyName)
    {
        if (!properties.TryGetValue(propertyName, out var field))
        {
            var attributes = FieldAttributes.Assembly | FieldAttributes.Static | FieldAttributes.InitOnly;
            field = new(propertyName, attributes, weaver.PropertyChangedEventArgsReference);
            properties.Add(propertyName, field);
        }

        return field;
    }

    public void InjectType()
    {
        if (properties.Count == 0)
        {
            return;
        }

        var attributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
        var cctor = new MethodDefinition(".cctor", attributes, weaver.TypeSystem.VoidReference);

        foreach (var pair in properties.OrderBy(i => i.Key))
        {
            var propertyName = pair.Key;
            var eventArgsField = pair.Value;

            cacheTypeDefinition.Fields.Add(eventArgsField);

            cctor.Body.Instructions.Append(
                Instruction.Create(OpCodes.Ldstr, propertyName),
                Instruction.Create(OpCodes.Newobj, weaver.PropertyChangedEventConstructorReference),
                Instruction.Create(OpCodes.Stsfld, eventArgsField)
            );
        }

        cctor.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ret)
        );

        cacheTypeDefinition.Methods.Add(cctor);
        weaver.ModuleDefinition.Types.Add(cacheTypeDefinition);
    }

    ModuleWeaver weaver;
    TypeDefinition cacheTypeDefinition;
    Dictionary<string, FieldDefinition> properties = new();
}
