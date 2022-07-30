using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

record ClassContext(ClassDeclarationSyntax SyntaxDeclaration, INamedTypeSymbol TypeSymbol)
{
    public static readonly IEqualityComparer<ClassContext> FullNameComparer = new FullNameOfClassContextComparer();

    public ClassDeclarationSyntax SyntaxDeclaration { get; } = SyntaxDeclaration;

    public INamedTypeSymbol TypeSymbol { get; } = TypeSymbol;

    public string FullName = TypeSymbol.ToDisplayString(FullNameDisplayFormat);

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
