using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    IEnumerable<ExplicitOnPropertyChangedMethodDependency> GetExplicitOnPropertyChangedMethodDependencies(TypeNode notifyNode)
    {
        if (!InjectOnPropertyNameChanged || !InjectExplicitOnPropertyNameChanged)
            yield break;
        var properties = notifyNode.TypeDefinition.Properties.ToList();
        var methods = notifyNode.TypeDefinition.Methods;

        foreach (var method in methods.Where(x => !x.IsStatic))
        {
            var onPropertyChangedAttribute =
                method.CustomAttributes.GetAttribute("PropertyChanged.OnPropertyChangedAttribute");
            if (onPropertyChangedAttribute == null)
                continue;

            if (method.ReturnType.FullName != "System.Void")
            {
                var message = string.Format("The type {0} has a method with an OnPropertyChanged attribute ({1}) that has a non void return value. Please make the return type void.", notifyNode.TypeDefinition.FullName, method.Name);
                throw new WeavingException(message);
            }

            var propertyDefinitions = GetOnPropertyChangedAttributeProperties(onPropertyChangedAttribute, method, properties);

            OnChangedTypes onChangedType;
            if (IsNoArgOnChangedMethod(method))
            {
                onChangedType = OnChangedTypes.NoArg;
            }
            else if (IsBeforeAfterOnChangedMethod(method))
            {
                onChangedType = OnChangedTypes.BeforeAfter;
            }
            else
            {
                var message = string.Format("The type {0} has a method with an OnPropertyChanged attribute ({1}) that has invalid parameter types. Only parameterless methods and methods with two System.Object parameters are supported.", notifyNode.TypeDefinition.FullName, method.Name);
                throw new WeavingException(message);
            }
            var typeDefinitions = new Stack<TypeDefinition>();
            typeDefinitions.Push(notifyNode.TypeDefinition);
            var methodReference = GetMethodReference(typeDefinitions, method);
            foreach (var propertyDefinition in propertyDefinitions)
            {
                yield return new ExplicitOnPropertyChangedMethodDependency
                {
                    OnChangedType = onChangedType,
                    WhenPropertyIsSet = propertyDefinition,
                    ShouldCallMethod = methodReference,
                };
            }
        }
    }
}