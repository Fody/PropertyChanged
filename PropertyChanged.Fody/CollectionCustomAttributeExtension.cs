using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    static string AssemblyVersion = typeof(ModuleWeaver).Assembly.GetName().Version.ToString();
    static string AssemblyName = typeof(ModuleWeaver).Assembly.GetName().Name;

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
        var attribute = new CustomAttribute(GeneratedCodeAttributeConstructor);
        attribute.ConstructorArguments.Add(new CustomAttributeArgument(TypeSystem.StringReference, AssemblyName));
        attribute.ConstructorArguments.Add(new CustomAttributeArgument(TypeSystem.StringReference, AssemblyVersion));
        customAttributes.Add(attribute);
    }
}