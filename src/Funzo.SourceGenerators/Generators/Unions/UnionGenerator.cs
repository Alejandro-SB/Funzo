using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sorse;
using Sorse.BuilderInterfaces;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators.Generators.Unions;

internal class UnionGenerator : GeneratorBase
{
    internal override string? GetSource(SourceProductionContext context, MarkedType type)
    {
        if (HasErrors(context, type))
        {
            return null;
        }

        var commonProperties = GetCommonProperties(type.AttributeTypeArguments);

        var classSymbol = type.Symbol;
        var className = $"{classSymbol.Name}";
        var typeArguments = type.AttributeTypeArguments;

        var sorse = WithSorse.CreateNamespace(classSymbol.ContainingNamespace.ToDisplayString(), []);

        sorse.AddClassWithInnerClasses(type, AddUnionClass(type));

        var src = sorse.GetSource();

        return src;
    }

    private Action<IClassBuilder> AddUnionClass(MarkedType type) =>
        builder =>
        {
            var typeArguments = type.AttributeTypeArguments;
            builder.Partial()
                .Inherits($"global::{FunzoAttributeSources.AttributeNamespace}.Union{typeArguments.OpenGenericPart()}");

            foreach (var typeArgument in typeArguments)
            {
                var typeName = typeArgument.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                builder.Implements($"global::Funzo.IUnion<{typeName}>")
                    .WithMethod("TUnion", "From", m =>
                                                    m.Static()
                                                    .WithGeneric("TUnion", g => g.WithClassConstraint().WithTypeConstraint($"IUnion<{typeName}>"))
                                                    .WithArguments([new(typeArgument, "_")])
                                                    .WithBody($" => (new {type.Symbol.Name}(_) as TUnion)!;"));
                builder.WithConstructor(c => c.WithBaseCall(["_"]).WithArguments([new(typeArgument, "_")]));
                builder.WithImplicitConversionOperatorFrom(typeArgument, " => new(x);");
            }

            var commonProperties = GetCommonProperties(type.AttributeTypeArguments);

            foreach (var prop in commonProperties)
            {
                builder.WithProperty(prop.Type, prop.Name, p => p.WithComputedValue($" => Match({string.Join(",", Enumerable.Range(0, typeArguments.Length).Select(_ => $"x => x.{prop.Name}"))});"));
            }
        };

    private bool HasErrors(SourceProductionContext context, MarkedType type)
    {
        var symbol = type.Symbol;

        if (!IsSymbolTopLevelOrInsidePartialClasses(symbol))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Union.TopLevelError, symbol);
            return true;
        }

        if (IsSymbolInheritingFromOtherClasses(symbol))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Union.WrongBaseType, symbol);
            return true;
        }


        if (IsAnyTypeArgumentObject(type.AttributeTypeArguments))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Union.ObjectNotValidType, symbol);
            return true;
        }

        if (IsAnyTypeArgumentAnInterface(type.AttributeTypeArguments))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Union.InterfaceNotValidType, symbol);
            return true;
        }

        if (IsAnyTypeRepeatedInUnion(type.AttributeTypeArguments))
        {
            CreateDiagnosticError(context, FunzoDiagnosticDescriptors.Union.RepeatedTypeSymbols, symbol);
            return true;
        }

        return false;
    }

    private static ImmutableArray<IPropertySymbol> GetCommonProperties(ImmutableArray<ITypeSymbol> typeArguments)
    {
        var typeProperties = typeArguments.SelectMany(t => t.GetPublicProperties()).ToLookup(x => x.Name);

        var commonProperties = typeProperties.Where(g => g.Count() == typeArguments.Length && g.All(s => s.Type.Equals(g.First().Type, SymbolEqualityComparer.IncludeNullability)))
            .Select(x => x.First())
            .ToImmutableArray();

        return commonProperties;
    }

    private static bool IsSymbolTopLevelOrInsidePartialClasses(INamedTypeSymbol type)
        => !(type.ContainingSymbol.Equals(type.ContainingNamespace, SymbolEqualityComparer.Default) && type.ContainingType is { } containerType
            && containerType.DeclaringSyntaxReferences.Any(syntax =>
                syntax.GetSyntax() is BaseTypeDeclarationSyntax declaration
                && !declaration.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword))));

    private static bool IsSymbolInheritingFromOtherClasses(INamedTypeSymbol symbol)
        => symbol.BaseType is not null && symbol.BaseType.Name != "Object";

    private static bool IsAnyTypeArgumentObject(ImmutableArray<ITypeSymbol> typeArguments)
        => typeArguments.Any(a => a.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "object");

    private static bool IsAnyTypeArgumentAnInterface(ImmutableArray<ITypeSymbol> typeArguments)
        => typeArguments.Any(t => t.TypeKind == TypeKind.Interface);

    private static bool IsAnyTypeRepeatedInUnion(ImmutableArray<ITypeSymbol> typeArguments)
        => typeArguments.ToLookup(x => x, SymbolEqualityComparer.Default).Any(g => g.Count() > 1);
}