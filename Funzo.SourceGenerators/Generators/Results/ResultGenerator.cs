using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

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

    internal abstract string ClassDefinition { get; }
    internal abstract string OkConstructor { get; }
    internal abstract string ErrConstructor { get; }
    internal abstract string OkStaticHelper { get; }
    internal abstract string ErrStaticHelper { get; }
    internal abstract string OkImplicitConverter { get; }
    internal abstract string ErrImplicitConverter { get; }

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

    protected bool TryGetImplicitConvertersForUnionType(ITypeSymbol type, ResultParameterType parameterType, out string converters)
    {
        if (!TryGetUnionTypes(type, out var unionTypes))
        {
            converters = string.Empty;
            return false;
        }

        var sb = new StringBuilder();

        var ctor = parameterType is ResultParameterType.Ok ? "Ok" : "Err";

        foreach (var unionType in unionTypes)
        {
            sb.AppendLine($@"public static implicit operator {ClassName}({unionType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} _) => {ctor}(_);");
        }

        converters = sb.ToString();
        return true;
    }
}
