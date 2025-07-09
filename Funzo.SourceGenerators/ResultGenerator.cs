using Funzo.SourceGenerators.Generators;
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
    public class ResultGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{FunzoAttributeSources.ResultAttributeName}.g.cs", FunzoAttributeSources.ResultAttributeContent));

            IncrementalValueProvider<ImmutableArray<SymbolWithAttribute>> resultClasses = context.SyntaxProvider
                .CreateSyntaxProvider(IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration(FunzoAttributeSources.ResultAttributeFullNames))
                .Where(static m => m is not null)
                .Collect()!;

            context.RegisterSourceOutput(resultClasses, (spc, symbols) => Execute(spc, symbols, new ResultSourceGenerator()));

            static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken _)
                => node is ClassDeclarationSyntax classDeclarationSyntax
                       && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

            static Func<GeneratorSyntaxContext, CancellationToken, SymbolWithAttribute?> GetSemanticTargetForGeneration(string[] attributeNames)
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

        private static void Execute(SourceProductionContext context, ImmutableArray<SymbolWithAttribute> symbols, SourceGeneratorBase sourceGenerator)
        {
            foreach (var symbol in symbols)
            {
                var source = sourceGenerator.GetSource(context, symbol);

                if (source is null)
                {
                    continue;
                }

                context.AddSource($"{symbol.Symbol!.ContainingNamespace}_{symbol.Symbol.Name}.g.cs", source);
            }
        }
    }
}