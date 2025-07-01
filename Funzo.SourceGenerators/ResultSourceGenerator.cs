using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators;

internal abstract class ResultSourceGenerator
{
    protected readonly INamedTypeSymbol ClassSymbol;
    protected readonly ImmutableArray<ITypeSymbol> TypeArguments;

    protected ResultSourceGenerator(INamedTypeSymbol classSymbol, ImmutableArray<ITypeSymbol> typeArguments)
    {
        ClassSymbol = classSymbol;
        TypeArguments = typeArguments;
    }

    internal abstract string ClassDefinition { get; }
    internal abstract string OkConstructor { get; }
    internal abstract string ErrConstructor { get; }
    internal abstract string OkStaticHelper { get; }
    internal abstract string ErrStaticHelper { get; }
    internal abstract string OkImplicitConverter { get; }
    internal abstract string ErrImplicitConverter { get; }

    protected string ClassNameWithGenerics => $"{ClassSymbol.Name}{OpenGenericPart()}";

    protected string? OpenGenericPart()
    {
        if (!ClassSymbol.TypeArguments.Any())
        {
            return null;
        }

        return $"<{GetGenerics(ClassSymbol.TypeArguments)}>";
    }

    private static string GetGenerics(ImmutableArray<ITypeSymbol> typeArguments) =>
        string.Join(", ", typeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
}
