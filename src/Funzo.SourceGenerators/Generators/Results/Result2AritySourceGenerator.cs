using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Sorse;
using Sorse.BuilderInterfaces;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators.Generators.Results;

internal class Result2AritySourceGenerator : ResultGenerator
{
    internal Result2AritySourceGenerator(MarkedType symbolWithAttribute) : base(symbolWithAttribute)
    {
    }

    protected override void GenerateResultInner(IClassBuilder builder)
    {
        builder.Inherits($"global::Funzo.ResultBase<{ClassName}, {OkDisplayName}, {ErrDisplayName}>")
            .Implements($"global::Funzo.IResultBase<{ClassName}, {OkDisplayName}, {ErrDisplayName}>")
            .WithConstructor(c => c.WithAccessModifier(AccessModifier.Protected)
                                    .WithArguments([new(OkType, "_")]).WithBaseCall(["_"]))
            .WithConstructor(c => c.WithAccessModifier(AccessModifier.Protected)
                                    .WithArguments([new(ErrType, "_")]).WithBaseCall(["_"]))
            .WithMethod(ClassName, "Ok", m => m.Static().WithArguments([new(OkType, "ok")]).WithBody(" => new(ok);"))
            .WithMethod(ClassName, "Err", m => m.Static().WithArguments([new(ErrType, "err")]).WithBody(" => new(err);"))
            .WithImplicitConversionOperatorFrom(OkType, " => new(x);")
            .WithImplicitConversionOperatorFrom(ErrType, " => new(x);");

        AddConversionsForUnions(builder);
    }

    private void AddConversionsForUnions(IClassBuilder builder)
    {
        if (HasCollidingParameters())
        {
            return;
        }

        var okUnions = GetTypesNeedingImplicitConversions(OkType, ResultParameterType.Ok);
        var errUnions = GetTypesNeedingImplicitConversions(ErrType, ResultParameterType.Err);

        foreach (var ok in okUnions)
        {
            builder.WithImplicitConversionOperatorFrom(ok, " => new(x);");
        }

        foreach (var err in errUnions)
        {
            builder.WithImplicitConversionOperatorFrom(err, " => new(x);");
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
