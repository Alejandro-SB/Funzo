using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Funzo.SourceGenerators;
internal class Result1AritySourceGenerator : ResultSourceGenerator
{
    internal Result1AritySourceGenerator(INamedTypeSymbol classSymbol, ImmutableArray<ITypeSymbol> typeArguments) : base(classSymbol, typeArguments)
    {
    }

    internal override string ClassDefinition => $@"partial class {ClassNameWithGenerics} : ResultBase<{ClassSymbol.Name},{ErrDisplayName}>, IResultBase <{ClassSymbol.Name},{ErrDisplayName}>";

    internal override string OkConstructor => @$"protected {ClassSymbol.Name}() : base() {{}}";

    internal override string ErrConstructor => $@"protected {ClassSymbol.Name}({ErrDisplayName} _) : base(_) {{}}";

    internal override string OkStaticHelper => $@"public static {ClassNameWithGenerics} Ok() => new();";

    internal override string ErrStaticHelper => $@"public static {ClassNameWithGenerics} Err({ErrDisplayName} err) => new(err);";

    internal override string OkImplicitConverter => string.Empty;

    internal override string ErrImplicitConverter => $@"public static implicit operator {ClassNameWithGenerics}({ErrDisplayName} _) => new {ClassNameWithGenerics}(_);";

    private string ErrDisplayName => TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}
