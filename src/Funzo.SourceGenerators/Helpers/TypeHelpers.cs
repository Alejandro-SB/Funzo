using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators.Helpers;
internal static class TypeHelpers
{
    internal static string? OpenGenericPart(this ImmutableArray<ITypeSymbol> symbols)
    {
        return $"<{GetGenerics(symbols)}>";
    }

    private static string GetGenerics(ImmutableArray<ITypeSymbol> symbols) =>
        string.Join(", ", symbols.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
}
