using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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

    internal override string OkImplicitConverter
    {
        get
        {
            var implicitConversions = new StringBuilder();
            implicitConversions.AppendLine($@"public static implicit operator {ClassNameWithGenerics}({OkDisplayName} _) => new {ClassNameWithGenerics}(_);");

            if (!HasCollidingParameters() && TryGetImplicitConvertersForUnionType(OkType, ResultParameterType.Ok, out var converters))
            {
                implicitConversions.AppendLine(converters);
            }

            return implicitConversions.ToString();
        }
    }

    internal override string ErrImplicitConverter
    {
        get
        {
            var implicitConversions = new StringBuilder();
            implicitConversions.AppendLine($@"public static implicit operator {ClassNameWithGenerics}({ErrDisplayName} _) => new {ClassNameWithGenerics}(_);");

            if (!HasCollidingParameters() && TryGetImplicitConvertersForUnionType(ErrType, ResultParameterType.Err, out var converters))
            {
                implicitConversions.AppendLine(converters);
            }

            return implicitConversions.ToString();
        }
    }

    private string OkDisplayName => OkType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    private string ErrDisplayName => ErrType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private ITypeSymbol OkType => TypeArguments[0];
    private ITypeSymbol ErrType => TypeArguments[1];

    /// <summary>
    /// Returns whether this error instance has colliding parameters in its Ok or Err parameters to avoid generating the implicit conversions
    /// </summary>
    /// <returns></returns>
    private bool HasCollidingParameters()
    {
        if (!TryGetUnionTypes(OkType, out var okTypes))
        {
            okTypes = [OkType];
        }

        if (!TryGetUnionTypes(ErrType, out var errTypes))
        {
            errTypes = [ErrType];
        }

        var hasCollisionInParameter = okTypes.Any(ok => errTypes.Contains(ok, SymbolEqualityComparer.Default));

        return hasCollisionInParameter;
    }
}
