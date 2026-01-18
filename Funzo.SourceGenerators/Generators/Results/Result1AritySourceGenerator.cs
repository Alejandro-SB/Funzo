using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Funzo.SourceGenerators.Generators.Results;
internal class Result1AritySourceGenerator : ResultGenerator
{
    internal Result1AritySourceGenerator(MarkedType symbolWithAttribute) : base(symbolWithAttribute)
    {
    }

    internal override string ClassDefinition => $@"partial class {ClassName} : global::Funzo.ResultBase<{ClassName},{ErrDisplayName}>, global::Funzo.IResultBase <{ClassName},{ErrDisplayName}>";

    internal override string OkConstructor => @$"protected {ClassName}() : base() {{}}";

    internal override string ErrConstructor => $@"protected {ClassName}({ErrDisplayName} _) : base(_) {{}}";

    internal override string OkStaticHelper => $@"public static {ClassName} Ok() => new();";

    internal override string ErrStaticHelper => $@"public static {ClassName} Err({ErrDisplayName} err) => new(err);";

    internal override string OkImplicitConverter => string.Empty;

    internal override string ErrImplicitConverter
    {
        get
        {
            var implicitConversions = new StringBuilder();
            implicitConversions.AppendLine($@"public static implicit operator {ClassName}({ErrDisplayName} _) => new {ClassName}(_);");

            if (TryGetImplicitConvertersForUnionType(ErrType, ResultParameterType.Err, out var converters))
            {
                implicitConversions.AppendLine(converters);
            }

            return implicitConversions.ToString();
        }
    }

    private string ErrDisplayName => ErrType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private ITypeSymbol ErrType => TypeArguments[0];
}
