using Microsoft.CodeAnalysis;

record TypeContext
{
    public static readonly IEqualityComparer<TypeContext> FullNameComparer = new FullNameOfTypeContextComparer();

    public TypeContext(INamedTypeSymbol typeSymbol)
    {
        ContainingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(FullNameDisplayFormat);

        ContainingTypeDeclarations = string.Join("|", typeSymbol.EnumerateContainingTypeDeclarations().Reverse().ToArray());

        FullName = typeSymbol.ToDisplayString(FullNameDisplayFormat);

        Declaration = $"{typeSymbol.GetTypeKeyword()} {typeSymbol.ToDisplayString(NameDisplayFormat)}";

        IsSealed = typeSymbol.IsSealed;

        HasBase = typeSymbol.Interfaces.Any(item => item.ToDisplayString() == "System.ComponentModel.INotifyPropertyChanged");
    }

    public string? ContainingNamespace { get; }

    public string ContainingTypeDeclarations { get; }

    string FullName { get; }

    public string Declaration { get; }

    public bool IsSealed { get; }

    public bool HasBase { get; }

    class FullNameOfTypeContextComparer : IEqualityComparer<TypeContext>
    {
        public bool Equals(TypeContext? x, TypeContext? y)
        {
            return string.Equals(x?.FullName, y?.FullName);
        }

        public int GetHashCode(TypeContext? obj)
        {
            return obj?.FullName.GetHashCode() ?? 0;
        }
    }
}
