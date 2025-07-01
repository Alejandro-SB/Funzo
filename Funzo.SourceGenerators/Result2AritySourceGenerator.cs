using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Funzo.SourceGenerators;
internal class Result2AritySourceGenerator : ResultSourceGenerator
{
    internal Result2AritySourceGenerator(INamedTypeSymbol classSymbol, ImmutableArray<ITypeSymbol> typeArguments) : base(classSymbol, typeArguments)
    {
    }

    internal override string ClassDefinition => $@"partial class {ClassNameWithGenerics} : ResultBase<{ClassSymbol.Name}, {OkDisplayName},{ErrDisplayName}>, IResultBase<{ClassSymbol.Name}, {OkDisplayName},{ErrDisplayName}>";

    internal override string OkConstructor => @$"protected {ClassSymbol.Name}({OkDisplayName} _) : base(_) {{}}";

    internal override string ErrConstructor => $@"protected {ClassSymbol.Name}({ErrDisplayName} _) : base(_) {{}}";

    internal override string OkStaticHelper => $@"public static {ClassNameWithGenerics} Ok({OkDisplayName} ok) => new(ok);";

    internal override string ErrStaticHelper => $@"public static {ClassNameWithGenerics} Err({ErrDisplayName} err) => new(err);";

    internal override string OkImplicitConverter => $@"public static implicit operator {ClassNameWithGenerics}({OkDisplayName} _) => new {ClassNameWithGenerics}(_);";

    internal override string ErrImplicitConverter => $@"public static implicit operator {ClassNameWithGenerics}({ErrDisplayName} _) => new {ClassNameWithGenerics}(_);";

    private string OkDisplayName => TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    private string ErrDisplayName => TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}
