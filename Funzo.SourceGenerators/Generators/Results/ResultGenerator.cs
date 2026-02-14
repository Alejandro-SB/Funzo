using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Sorse.BuilderInterfaces;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Funzo.SourceGenerators.Generators.Results;

internal abstract class ResultGenerator
{
    private readonly MarkedType _symbolWithAttribute;
    protected INamedTypeSymbol ClassSymbol => _symbolWithAttribute.Symbol;
    protected ImmutableArray<ITypeSymbol> TypeArguments => _symbolWithAttribute.AttributeTypeArguments;

    protected ResultGenerator(MarkedType symbolWithAttribute)
    {
        _symbolWithAttribute = symbolWithAttribute;
    }

    internal void GenerateResult(IClassBuilder builder)
    {
        builder.Partial();

        GenerateResultInner(builder);
    }

    protected abstract void GenerateResultInner(IClassBuilder builder);
    protected string ClassName => $"{ClassSymbol.Name}";

    protected bool TryGetUnionTypes(ITypeSymbol type, out IEnumerable<ITypeSymbol> types)
    {
        var unionTypeArguments = type.GetUnionTypeArguments();

        if (unionTypeArguments is null)
        {
            types = [];
            return false;
        }
        else
        {
            types = unionTypeArguments;
            return true;
        }
    }

    protected IEnumerable<ITypeSymbol> GetTypesNeedingImplicitConversions(ITypeSymbol type, ResultParameterType parameterType)
    {
        if (!TryGetUnionTypes(type, out var unionTypes))
        {
            return [];
        }

        return unionTypes;
    }
}
