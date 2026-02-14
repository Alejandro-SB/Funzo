using Funzo.SourceGenerators.Generators.Results;
using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sorse;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators.Generators;

internal class ResultSourceGenerator : GeneratorBase
{
    internal override string? GetSource(SourceProductionContext context, MarkedType type)
    {
        if (HasErrors(context, type))
        {
            return null;
        }

        var is1ArityResult = type.AttributeTypeArguments.Length == 1;


        ResultGenerator generator = is1ArityResult
            ? new Result1AritySourceGenerator(type)
            : new Result2AritySourceGenerator(type);

        var sorse = WithSorse.CreateNamespaceScope(type.Symbol.ContainingNamespace.ToDisplayString(), []);

        sorse.AddClassWithInnerClasses(type, generator.GenerateResult);

        var src = sorse.GetSource();

        return src;
    }

    private bool HasErrors(SourceProductionContext context, MarkedType type)
    {
        var symbol = type.Symbol;
        var typeArguments = type.AttributeTypeArguments;

        if (!IsSymbolTopLevelOrInsidePartialClasses(symbol))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Result.TopLevelError, symbol);
            return true;
        }

        if (IsSymbolInheritingFromOtherClasses(symbol))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Result.WrongBaseType, symbol);
            return true;
        }

        if (IsAnyTypeArgumentObject(typeArguments))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Result.ObjectNotValidType, symbol);
            return true;
        }

        if (AreTypeArgumentsTheSame(typeArguments))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Result.RepeatedTypeSymbols, symbol);
            return true;
        }

        return false;
    }

    private static bool IsSymbolTopLevelOrInsidePartialClasses(INamedTypeSymbol symbol)
        => !(!symbol.ContainingSymbol.Equals(symbol.ContainingNamespace, SymbolEqualityComparer.Default) && symbol.ContainingType is { } containerType
           && containerType.DeclaringSyntaxReferences.Any(syntax =>
               syntax.GetSyntax() is BaseTypeDeclarationSyntax declaration
               && !declaration.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword))));

    private static bool IsSymbolInheritingFromOtherClasses(INamedTypeSymbol symbol)
        => symbol.BaseType is not null && symbol.BaseType.Name != "Object";

    private static bool IsAnyTypeArgumentObject(ImmutableArray<ITypeSymbol> typeArguments)
        => typeArguments.Any(a => a.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "object");

    private static bool AreTypeArgumentsTheSame(ImmutableArray<ITypeSymbol> typeArguments)
        => typeArguments.Count() == 2 && SymbolEqualityComparer.Default.Equals(typeArguments.First(), typeArguments.Last());
}
