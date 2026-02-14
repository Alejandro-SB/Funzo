using Sorse;
using Sorse.BuilderInterfaces;
using System;
using System.Linq;

namespace Funzo.SourceGenerators.Helpers;

internal static class SorseExtensions
{
    internal static void AddClassWithInnerClasses(this INamespaceScope sorse, MarkedType type, Action<IClassBuilder> builderAction)
    {
        var classSymbol = type.Symbol;
        var className = $"{classSymbol.Name}";
        var typeArguments = type.AttributeTypeArguments;

        // TODO: Needs some rework. Feels off
        if (!type.ContainerClasses.Any())
        {
            sorse.AddClass(className, builderAction);
        }
        else
        {
            var builder = GetClassBuilderFromContainingClasses(type, sorse);

            // Add the main partial class
            builder.WithInnerClass(className, builderAction);
        }
    }

    private static IClassBuilder GetClassBuilderFromContainingClasses(MarkedType type, INamespaceScope sorse)
    {
        // Create the first class in the namespace
        var container = type.ContainerClasses.Pop();

        IClassBuilder builder = null!;

        sorse.AddClass(container.Name, b =>
        {
            builder = b;
            AddContainer(b, container);
        });

        // Nest the rest
        foreach (var c in type.ContainerClasses)
        {
            builder.WithInnerClass(c.Name, b =>
            {
                builder = b;
                AddContainer(b, c);
            });

        }

        return builder;

        static void AddContainer(IClassBuilder builder, ContainerClass c)
        {
            builder.Partial();

            if (c.IsStatic)
            {
                builder.Static();
            }
        }
    }
}
