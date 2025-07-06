using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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

    protected bool TryGetUnionTypes(ITypeSymbol type, out IEnumerable<ITypeSymbol> types)
    {
        var isUnion = type.BaseType is ITypeSymbol baseType && baseType.Name == "Union" && baseType.ContainingNamespace.Name == "Funzo";

        if (isUnion)
        {
            types = type.BaseType!.TypeArguments;
        }
        else
        {
            types = [];
        }

        return isUnion;
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
            sb.AppendLine($@"public static implicit operator {ClassNameWithGenerics}({unionType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} _) => {ctor}(_);");
        }

        converters = sb.ToString();
        return true;
    }
}
