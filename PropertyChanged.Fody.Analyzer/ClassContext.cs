using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

record ClassContext(ClassDeclarationSyntax SyntaxDeclaration, INamedTypeSymbol TypeSymbol)
{
    public ClassDeclarationSyntax SyntaxDeclaration { get; } = SyntaxDeclaration;
    public INamedTypeSymbol TypeSymbol { get; } = TypeSymbol;
}