using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    private static void FixupConstructorAutoPropertyInitializers(TypeNode node)
    {
        /* 
         * Initializing auto-properties in the constructor(s) will implicityl generate 
         * a call to the virtual "OnPropertyChanged" methdod after the class is weaved,
         * which will generate a CA2214:DoNotCallOverridableMethodsInConstructors:
         * 
         *  [ImplementPropertyChanged]
         *  public class Class
         *  {
         *      public Class(string value)
         *      {
         *          Property = value;
         *      }
         *
         *      public string Property { get; set; }
         *      
         *      public bool IsChanged { get; set; }
         *  }
         *  
         * Another annoying side effect is that if the class has an IsChanged property, 
         * IsChanged returns true right after creating the object.
         * 
         * Usually you would initialize the backing field instead of the property, 
         * but with auto-properties there is no way to access the backing field, and converting
         * all auto-properties to properties with backing fields would make PropertyChanged.Fody nearly obsolete.
         * 
         * To avoid this issue, replace all auto-property setter calls in constructors with a setter of the backing field.
         * 
         * This will however not catch initializing auto-properties from methods called by the constructor.
         */

        foreach (var ctor in node.TypeDefinition.Methods.Where(method => method.IsConstructor && method.HasBody))
        {
            var instructions = ctor.Body.Instructions;

            for (var index = 0; index < instructions.Count; index++)
            {
                var instruction = instructions[index];

                if (!IsPropertySetterCall(instruction, out string propertyName))
                    continue;

                var property = node.PropertyDatas.FirstOrDefault(item => string.Equals(item.PropertyDefinition.Name, propertyName, StringComparison.Ordinal));

                var backingField = property?.BackingFieldReference;

                var customAttributes = backingField?.Resolve()?.CustomAttributes;

                if (true != customAttributes?.Any(item => item.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                    continue;

                //if (backingField?.Name != $"<{propertyName}>k__BackingField")
                //    continue;

                instructions[index] = Instruction.Create(OpCodes.Stfld, backingField);
            }
        }
    }

    private static bool IsPropertySetterCall(Instruction instruction, out string propertyName)
    {
        propertyName = null;

        if (instruction.OpCode.Code != Code.Call)
            return false;

        var operand = instruction.Operand as MethodDefinition;
        if (operand == null)
            return false;

        if (!operand.IsSetter)
            return false;

        var operandName = operand.Name;
        if (!operandName.StartsWith("set_"))
            return false;

        propertyName = operandName.Substring(4);
        return true;
    }
}
