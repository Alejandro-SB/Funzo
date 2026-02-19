using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Sorse.BuilderInterfaces;

namespace Funzo.SourceGenerators.Generators.Results;

internal class Result1AritySourceGenerator : ResultGenerator
{
    internal Result1AritySourceGenerator(MarkedType symbolWithAttribute) : base(symbolWithAttribute)
    {
    }

    protected override void GenerateResultInner(IClassBuilder builder)
    {
        builder.Inherits($"global::Funzo.ResultBase<{ClassName}, {ErrDisplayName}>")
            .Implements($"global::Funzo.IResultBase<{ClassName}, {ErrDisplayName}>")
            .WithConstructor(c => c.WithBaseCall([]))
            .WithConstructor(c => c.WithArguments([new(ErrType, "x")]).WithBaseCall(["x"]))
            .WithMethod(ClassName, "Ok", m => m.Static().WithBody(" => new();"))
            .WithMethod(ClassName, "Err", m => m.Static().WithArguments([new(ErrType, "x")]).WithBody(" => new(x);"))
            .WithImplicitConversionOperatorFrom(ErrType, " => new(x);");

        AddConversionsForErrUnions(builder);
    }

    private void AddConversionsForErrUnions(IClassBuilder builder)
    {
        foreach (var type in GetTypesNeedingImplicitConversions(ErrType))
        {
            builder.WithImplicitConversionOperatorFrom(type, $" => new(x);");
        }
    }

    private string ErrDisplayName => ErrType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private ITypeSymbol ErrType => TypeArguments[0];

}
