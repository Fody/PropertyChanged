using Microsoft.CodeAnalysis;

record ClassContext
{
    public static readonly IEqualityComparer<ClassContext> FullNameComparer = new FullNameOfClassContextComparer();

    public ClassContext(INamedTypeSymbol typeSymbol)
    {
        ContainingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(FullNameDisplayFormat);

        ContainingTypeNames = string.Join("|", typeSymbol.EnumerateContainingTypeNames().Reverse().ToArray());

        FullName = typeSymbol.ToDisplayString(FullNameDisplayFormat);

        Name = typeSymbol.ToDisplayString(NameDisplayFormat);

        IsSealed = typeSymbol.IsSealed;

        HasBase = typeSymbol.Interfaces.Any(item => item.ToDisplayString() == "System.ComponentModel.INotifyPropertyChanged");
    }

    public string? ContainingNamespace { get; }

    public string ContainingTypeNames { get; }

    string FullName { get; }

    public string Name { get; }

    public bool IsSealed { get; }

    public bool HasBase { get; }

    class FullNameOfClassContextComparer : IEqualityComparer<ClassContext>
    {
        public bool Equals(ClassContext? x, ClassContext? y)
        {
            return string.Equals(x?.FullName, y?.FullName);
        }

        public int GetHashCode(ClassContext? obj)
        {
            return obj?.FullName.GetHashCode() ?? 0;
        }
    }
}
