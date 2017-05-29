using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class IlGeneratedByDependencyReader
{
    TypeNode node;
    Func<MethodReference, MethodDefinition, bool> methodComparer;
    Func<FieldReference, FieldDefinition, bool> fieldComparer;

    public IlGeneratedByDependencyReader(TypeNode node)
    {
        this.node = node;
    }

    public void Process()
    {
        if (node.TypeDefinition.HasGenericParameters)
        {
            methodComparer = GenericMethodComparer;
            fieldComparer = GenericFieldComparer;
        }
        else
        {
            methodComparer = NonGenericMethodComparer;
            fieldComparer = NonGenericFieldComparer;
        }
        foreach (var property in node.TypeDefinition.Properties)
        {
            if (!property.CustomAttributes.ContainsAttribute("PropertyChanged.DoNotNotifyAttribute"))
            {
                ProcessGet(property);
            }
        }
    }


    static bool GenericMethodComparer(MethodReference methodReference, MethodDefinition methodDefinition)
    {
        return methodDefinition == methodReference.Resolve();
    }
    static bool NonGenericMethodComparer(MethodReference methodReference, MethodDefinition methodDefinition)
    {
        return methodDefinition == methodReference;
    }
    static bool GenericFieldComparer(FieldReference fieldReference, FieldDefinition fieldDefinition)
    {
        return fieldDefinition == fieldReference.Resolve();
    }
    static bool NonGenericFieldComparer(FieldReference fieldReference, FieldDefinition fieldDefinition)
    {
        return fieldDefinition == fieldReference;
    }
    void ProcessGet(PropertyDefinition property)
    {
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
        PropertyDefinition usedProperty;
        if (IsPropertyGetInstruction(instruction, out usedProperty) || IsFieldGetInstruction(instruction, out usedProperty))
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
            var methodReference = instruction.Operand as MethodReference;
            if (methodReference != null)
            {
                var mapping = node.Mappings.FirstOrDefault(x => methodComparer(methodReference, x.PropertyDefinition.GetMethod));
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
            var fieldReference = instruction.Operand as FieldReference;
            if (fieldReference != null)
            {
                var mapping = node.Mappings.FirstOrDefault(x => fieldComparer(fieldReference, x.FieldDefinition));
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