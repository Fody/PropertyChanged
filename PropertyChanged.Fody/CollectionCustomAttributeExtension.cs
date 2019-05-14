using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void MarkAsGeneratedCode(Collection<CustomAttribute> customAttributes)
    {
        AddCustomAttributeArgument(customAttributes);
        AddDebuggerNonUserCodeAttribute(customAttributes);
    }

    void AddDebuggerNonUserCodeAttribute(Collection<CustomAttribute> customAttributes)
    {
        var debuggerAttribute = new CustomAttribute(DebuggerNonUserCodeAttributeConstructor);
        customAttributes.Add(debuggerAttribute);
    }

    void AddCustomAttributeArgument(Collection<CustomAttribute> customAttributes)
    {
        var version = typeof(ModuleWeaver).Assembly.GetName().Version.ToString();
        var name = typeof(ModuleWeaver).Assembly.GetName().Name;

        var attribute = new CustomAttribute(GeneratedCodeAttributeConstructor);
        attribute.ConstructorArguments.Add(new CustomAttributeArgument(TypeSystem.StringReference, name));
        attribute.ConstructorArguments.Add(new CustomAttributeArgument(TypeSystem.StringReference, version));
        customAttributes.Add(attribute);
    }
}