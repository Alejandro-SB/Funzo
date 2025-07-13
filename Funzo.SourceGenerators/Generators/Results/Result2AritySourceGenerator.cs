using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Funzo.SourceGenerators.Generators.Results;
internal class Result2AritySourceGenerator : ResultGenerator
{
    internal Result2AritySourceGenerator(SymbolWithAttribute symbolWithAttribute) : base(symbolWithAttribute)
    {
    }

    internal override string ClassDefinition => $@"partial class {ClassName} : global::Funzo.ResultBase<{ClassName}, {OkDisplayName},{ErrDisplayName}>, global::Funzo.IResultBase<{ClassName}, {OkDisplayName},{ErrDisplayName}>";

    internal override string OkConstructor => @$"protected {ClassName}({OkDisplayName} _) : base(_) {{}}";

    internal override string ErrConstructor => $@"protected {ClassName}({ErrDisplayName} _) : base(_) {{}}";

    internal override string OkStaticHelper => $@"public static {ClassName} Ok({OkDisplayName} ok) => new(ok);";

    internal override string ErrStaticHelper => $@"public static {ClassName} Err({ErrDisplayName} err) => new(err);";

    internal override string OkImplicitConverter
    {
        get
        {
            var implicitConversions = new StringBuilder();
            implicitConversions.AppendLine($@"public static implicit operator {ClassName}({OkDisplayName} _) => new {ClassName}(_);");

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
            implicitConversions.AppendLine($@"public static implicit operator {ClassName}({ErrDisplayName} _) => new {ClassName}(_);");

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
