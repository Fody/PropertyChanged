using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public List<string> NamespaceFilters { get; private set; }

    public void ProcessFilterTypeAttributes()
    {
        ReadFilterTypeData(ModuleDefinition);
    }

    public void ReadFilterTypeData(ModuleDefinition moduleDefinition)
    {
        var filterTypeAttribute = moduleDefinition.Assembly.CustomAttributes.GetAttributes("PropertyChanged.FilterTypeAttribute");
        if (filterTypeAttribute == null)
        {
            return;
        }
        var customAttributeArguments = filterTypeAttribute.Select(x => x.ConstructorArguments).ToList();
        NamespaceFilters = customAttributeArguments.Select(x => (string)x[0].Value).ToList();
    }
}