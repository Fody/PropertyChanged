using Mono.Cecil;

public class ExplicitOnPropertyChangedMethodDependency
{
    public PropertyDefinition WhenPropertyIsSet;
    public MethodReference ShouldCallMethod;
    public OnChangedTypes OnChangedType;
}