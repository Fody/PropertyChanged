using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public bool FoundInterceptor;
    public MethodDefinition InterceptMethod;
    public InvokerTypes InterceptorType;

    void SearchForMethod(TypeDefinition typeDefinition)
    {
        var methodDefinition = typeDefinition.Methods.FirstOrDefault(x => x.Name == "Intercept");
        if (methodDefinition == null)
        {
            throw new WeavingException(string.Format("Found Type '{0}' but could not find a method named 'Intercept'.", typeDefinition.FullName));
        }
        if (!methodDefinition.IsStatic)
        {
            throw new WeavingException(string.Format("Found Type '{0}.Intercept' but it is not static.", typeDefinition.FullName));
        }
        if (!methodDefinition.IsPublic)
        {
            throw new WeavingException(string.Format("Found Type '{0}.Intercept' but it is not public.", typeDefinition.FullName));
        }

        if (IsSingleStringInterceptionMethod(methodDefinition))
        {
            FoundInterceptor = true;
            InterceptMethod = methodDefinition;
            InterceptorType = InvokerTypes.String;
            return;
        }
        if (IsBeforeAfterInterceptionMethod(methodDefinition))
        {
            FoundInterceptor = true;
            InterceptMethod = methodDefinition;
            InterceptorType = InvokerTypes.BeforeAfter;
            return;
        }
        var message = string.Format(
            @"Found '{0}.Intercept' But the signature is not correct. It needs to be either.
Intercept(object target, Action firePropertyChanged, string propertyName)
or
Intercept(object target, Action firePropertyChanged, string propertyName, object before, object after)", typeDefinition.FullName);
        throw new WeavingException(message);
    }


    public bool IsSingleStringInterceptionMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 3
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String";
    }

    public bool IsBeforeAfterInterceptionMethod(MethodDefinition method)
    {
        var parameters = method.Parameters;
        return parameters.Count == 5
               && parameters[0].ParameterType.FullName == "System.Object"
               && parameters[1].ParameterType.FullName == "System.Action"
               && parameters[2].ParameterType.FullName == "System.String"
               && parameters[3].ParameterType.FullName == "System.Object"
               && parameters[4].ParameterType.FullName == "System.Object";
    }

    public void FindInterceptor()
    {
        var typeDefinition = ModuleDefinition.Types.FirstOrDefault(x => x.Name == "PropertyChangedNotificationInterceptor");
        if (typeDefinition == null)
        {
            FoundInterceptor = false;
            return;
        }
        SearchForMethod(typeDefinition);
    }
}