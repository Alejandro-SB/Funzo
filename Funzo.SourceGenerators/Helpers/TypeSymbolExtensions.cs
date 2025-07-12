using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Funzo.SourceGenerators.Helpers;
internal static class TypeSymbolExtensions
{
    internal static AttributeData? GetGenericAttributeFromNames(this ITypeSymbol typeSymbol, string[] attributeNames)
        => typeSymbol.GetAttributes()
                        .FirstOrDefault(a => attributeNames.Any(afn =>
                            a.AttributeClass is { } attributeClass
                            && attributeClass.IsGenericType
                            && string.Equals(attributeClass.ConstructUnboundGenericType().ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), $"global::{afn}")));

    internal static ImmutableArray<ITypeSymbol> GetGenericTypeArguments(this AttributeData attributeData)
        => attributeData.AttributeClass!.IsGenericType
            ? attributeData.AttributeClass.TypeArguments
            : ImmutableArray<ITypeSymbol>.Empty;

    internal static ImmutableArray<ITypeSymbol>? GetUnionTypeArguments(this ITypeSymbol type)
    {
        var subClassTypeArguments = GetUnionSubClassTypeArguments(type);

        if(subClassTypeArguments is not null)
        {
            return subClassTypeArguments;
        }

        var unionAttribute = type.GetGenericAttributeFromNames(FunzoAttributeSources.UnionAttributeFullNames);

        return unionAttribute is null
            ? null
            : unionAttribute.GetGenericTypeArguments();
    }

    internal static ImmutableArray<IPropertySymbol> GetPublicProperties(this ITypeSymbol symbol) 
        => symbol.GetMembers().OfType<IPropertySymbol>().Where(s => s.DeclaredAccessibility is Accessibility.Public).ToImmutableArray();

    private static ImmutableArray<ITypeSymbol>? GetUnionSubClassTypeArguments(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedSymbol && IsTypeUnion(type))
        {
            return namedSymbol.TypeArguments;
        }

        if (type.BaseType is { } baseSymbol && IsTypeUnion(baseSymbol))
        {
            return baseSymbol.TypeArguments;
        }

        return null;
    }

    private static bool IsTypeUnion(ITypeSymbol type)
    {
        return type.Name == "Union" && type.ContainingNamespace.Name == "Funzo";
    }
}
