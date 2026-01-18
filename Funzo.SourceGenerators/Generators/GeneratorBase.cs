using Funzo.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Funzo.SourceGenerators.Generators;

internal abstract class GeneratorBase
{
    protected GeneratorBase()
    {
    }

    protected void CreateDiagnosticError(SourceProductionContext context, DiagnosticDescriptor descriptor, INamedTypeSymbol classSymbol)
    {
        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, classSymbol.Name,
            DiagnosticSeverity.Error));
    }

    internal abstract string? GetSource(SourceProductionContext context, MarkedType type);
}
