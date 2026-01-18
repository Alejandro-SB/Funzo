using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Funzo.SourceGenerators.Helpers;

internal class MarkedType
{
    internal INamedTypeSymbol Symbol { get; }
    internal ImmutableArray<ITypeSymbol> AttributeTypeArguments { get; }
    internal Stack<ContainerClass> ContainerClasses { get; } = [];

    public MarkedType(INamedTypeSymbol symbol, AttributeData attributeData)
    {
        Symbol = symbol;
        AttributeTypeArguments = attributeData.AttributeClass!.TypeArguments;

        ContainerClasses = GenerateContainerClassesHierarchy(symbol);
    }

    private static Stack<ContainerClass> GenerateContainerClassesHierarchy(INamedTypeSymbol symbol)
    {
        var container = symbol.ContainingType;

        var containers = new Stack<ContainerClass>();

        while (container is not null)
        {
            containers.Push(new(container.Name));
            container = container.ContainingType;
        }

        return containers;
    }
}
