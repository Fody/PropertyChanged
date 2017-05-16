using System;
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;


public static class DefinitionFinder
{
    public static PropertyDefinition FindProperty<T>(Expression<Func<T>> expression)
    {
        var memberExpression = (MemberExpression)expression.Body;
        var declaringType = memberExpression.Member.DeclaringType;
        return FindType(declaringType, memberExpression.Member.Name);
    }

    public static PropertyDefinition FindProperty<T>(string name)
    {
        var typeDefinition = FindType(typeof(T));
        return typeDefinition.Properties.First(x => x.Name == name);
    }

    static PropertyDefinition FindType(Type declaringType, string name)
    {
        var typeDefinition = FindType(declaringType);
        return typeDefinition.Properties.First(x => x.Name == name);
    }

    public static MethodDefinition FindMethod<T>(Expression<Action> expression)
    {
        var callExpression = (MethodCallExpression)expression.Body;
        var declaringType = callExpression.Method.DeclaringType;

        var typeDefinition = FindType(declaringType);

        return typeDefinition.Methods.First(x => x.Name == callExpression.Method.Name);
    }

    public static TypeDefinition FindType<T>()
    {
        var declaringType = typeof(T);

        return FindType(declaringType);
    }

    static TypeDefinition FindType(Type typeToFind)
    {
        var targetPath = typeToFind.Assembly.CodeBase.Replace("file:///","");
        var assemblyResolver = new TestAssemblyResolver();
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = assemblyResolver
        };
        var moduleDefinition = ModuleDefinition.ReadModule(targetPath, readerParameters);

        foreach (var type in moduleDefinition.Types)
        {
            if (type.Name == typeToFind.Name)
            {
                return type;
            }
            foreach (var nestedType in type.NestedTypes)
            {
                if (nestedType.Name == typeToFind.Name && nestedType.DeclaringType.Name == typeToFind.DeclaringType.Name)
                {
                    return nestedType;
                }
            }
        }
        throw new Exception();
    }
}