using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Funzo.SourceGenerators.Helpers;

internal class SymbolWithAttribute
{
    internal INamedTypeSymbol Symbol { get; }
    internal AttributeData AttributeData { get; }
    internal ImmutableArray<ITypeSymbol> TypeArguments => AttributeData.AttributeClass!.TypeArguments;

    public SymbolWithAttribute(INamedTypeSymbol symbol, AttributeData resultAttribute)
    {
        Symbol = symbol;
        AttributeData = resultAttribute;
    }

    internal void Deconstruct(out INamedTypeSymbol symbol, out AttributeData resultAttribute)
    {
        symbol = Symbol;
        resultAttribute = AttributeData;
    }
}
