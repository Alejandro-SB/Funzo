using Funzo.SourceGenerators.Generators;
using Funzo.SourceGenerators.Generators.Unions;
using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Funzo.SourceGenerators
{
    [Generator]
    public class FunzoGenerators : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{FunzoAttributeSources.UnionAttributeName}.g.cs", FunzoAttributeSources.UnionAttributeContent));

            IncrementalValueProvider<ImmutableArray<MarkedType>> unionClasses = context.SyntaxProvider
                .CreateSyntaxProvider(IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration(FunzoAttributeSources.UnionAttributeFullNames))
                .Where(static m => m is not null)
                .Collect()!;

            context.RegisterSourceOutput(unionClasses, (spc, symbols) => Execute(spc, symbols, new UnionGenerator()));

            static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken _)
                => node is ClassDeclarationSyntax classDeclarationSyntax
                       && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

            static Func<GeneratorSyntaxContext, CancellationToken, MarkedType?> GetSemanticTargetForGeneration(string[] attributeNames)
                => (context, cancellationToken) =>
                {
                    var node = context.Node;

                    var symbol = context.SemanticModel.GetDeclaredSymbol(node);

                    if (symbol is not INamedTypeSymbol namedTypeSymbol)
                    {
                        return null;
                    }

                    var attribute = namedTypeSymbol.GetGenericAttributeFromNames(attributeNames);

                    return attribute is not null ? new(namedTypeSymbol, attribute) : null;
                };
        }

        private static void Execute(SourceProductionContext context, ImmutableArray<MarkedType> markedTypes, GeneratorBase sourceGenerator)
        {
            foreach (var type in markedTypes)
            {
                var source = sourceGenerator.GetSource(context, type);

                if (source is null)
                {
                    continue;
                }

                var containingNamespace = type.Symbol.ContainingNamespace + string.Join("_", type.ContainerClasses);

                context.AddSource($"{containingNamespace}_{type.Symbol.Name}.g.cs", source);
            }
        }
    }
}