using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class IlGeneratedByDependencyReader
{
    TypeNode node;

    public IlGeneratedByDependencyReader(TypeNode node)
    {
        this.node = node;
    }

    public void Process()
    {
        foreach (var property in node.TypeDefinition.Properties)
        {
            if (!property.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotNotifyAttribute"))
            {
                ProcessGet(property);
            }
        }
    }

    static bool MethodComparer(MethodReference methodReference, MethodDefinition methodDefinition)
    {
        return methodReference.Name == methodDefinition?.Name
            && CoreMethodComparer(methodReference.Resolve(), methodDefinition);
    }

    static bool CoreMethodComparer(MethodReference methodReference, MethodDefinition methodDefinition)
    {
        return methodDefinition.GetSelfAndBaseMethods().Any(item => item == methodReference);
    }

    static bool FieldComparer(FieldReference fieldReference, FieldDefinition fieldDefinition)
    {
        return fieldReference.Name == fieldDefinition?.Name
            && fieldReference.Resolve() == fieldDefinition;
    }

    void ProcessGet(PropertyDefinition property)
    {
        //Exclude indexers
        if (property.HasParameters)
        {
            return;
        }

        var getMethod = property.GetMethod;

        //Exclude when no get
        if (getMethod == null)
        {
            return;
        }
        //Exclude when abstract
        if (getMethod.IsAbstract)
        {
            return;
        }
        var recursiveIlFinder = new RecursiveIlFinder(property.DeclaringType);
        recursiveIlFinder.Execute(getMethod);
        foreach (var instruction in recursiveIlFinder.Instructions)
        {
            ProcessInstructionForGet(property, instruction);
        }
    }

    void ProcessInstructionForGet(PropertyDefinition property, Instruction instruction)
    {
        if (IsPropertyGetInstruction(instruction, out var usedProperty) ||
            IsFieldGetInstruction(instruction, out usedProperty))
        {
            if (usedProperty == property)
            {
                //skip where self reference
                return;
            }
            var dependency = new PropertyDependency
            {
                ShouldAlsoNotifyFor = property,
                WhenPropertyIsSet = usedProperty
            };
            node.PropertyDependencies.Add(dependency);
        }
    }

    public bool IsPropertyGetInstruction(Instruction instruction, out PropertyDefinition propertyDefinition)
    {
        if (instruction.OpCode.IsCall())
        {
            if (instruction.Operand is MethodReference methodReference)
            {
                var mapping = node.Mappings.FirstOrDefault(x => MethodComparer(methodReference, x.PropertyDefinition.GetMethod));
                if (mapping != null)
                {
                    propertyDefinition = mapping.PropertyDefinition;
                    return true;
                }
            }
        }
        propertyDefinition = null;
        return false;
    }

    public bool IsFieldGetInstruction(Instruction instruction, out PropertyDefinition propertyDefinition)
    {
        if (instruction.OpCode.Code == Code.Ldfld)
        {
            if (instruction.Operand is FieldReference fieldReference)
            {
                var mapping = node.Mappings.FirstOrDefault(x => FieldComparer(fieldReference, x.FieldDefinition));
                if (mapping != null)
                {
                    propertyDefinition = mapping.PropertyDefinition;
                    return true;
                }
            }
        }
        propertyDefinition = null;
        return false;
    }
}
