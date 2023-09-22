using Mono.Cecil;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    static readonly string AssemblyVersion = typeof(ModuleWeaver).Assembly.GetName().Version.ToString();
    static readonly string AssemblyName = typeof(ModuleWeaver).Assembly.GetName().Name;

    public void MarkAsGeneratedCode(Collection<CustomAttribute> customAttributes)
    {
        AddGeneratedCodeAttribute(customAttributes);
        AddDebuggerNonUserCodeAttribute(customAttributes);
    }

    void AddDebuggerNonUserCodeAttribute(Collection<CustomAttribute> customAttributes)
    {
        var debuggerAttribute = new CustomAttribute(DebuggerNonUserCodeAttributeConstructor);
        customAttributes.Add(debuggerAttribute);
    }

    void AddGeneratedCodeAttribute(Collection<CustomAttribute> customAttributes)
    {
        var attribute = new CustomAttribute(GeneratedCodeAttributeConstructor);
        attribute.ConstructorArguments.Add(new(TypeSystem.StringReference, AssemblyName));
        attribute.ConstructorArguments.Add(new(TypeSystem.StringReference, AssemblyVersion));
        customAttributes.Add(attribute);
    }
}