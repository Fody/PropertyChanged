using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class TypeNode
{
    public TypeDefinition TypeDefinition;
    public List<TypeNode> Nodes = new();
    public List<PropertyDependency> PropertyDependencies = new();
    public List<MemberMapping> Mappings = new();
    public EventInvokerMethod EventInvoker;
    public MethodReference IsChangedInvoker;
    public List<PropertyData> PropertyDatas = new();
    public List<PropertyDefinition> AllProperties;
    public ICollection<OnChangedMethod> OnChangedMethods;
    public IEnumerable<PropertyDefinition> DeclaredProperties => AllProperties.Where(prop => prop.DeclaringType == TypeDefinition);
}
