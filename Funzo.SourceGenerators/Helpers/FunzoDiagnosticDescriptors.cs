using Microsoft.CodeAnalysis;

namespace Funzo.SourceGenerators.Helpers;

internal static class FunzoDiagnosticDescriptors
{
    public static class Result
    {
        public static DiagnosticDescriptor TopLevelError
            => new("FNZ0001",
                "Class must be top level",
                "Class '{0}' using ResultGenerator must be top level or inside a partial class",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor WrongBaseType
            => new("FNZ0002", "Result should not have a base class",
                "Class '{0}' should not have any base class",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor ObjectNotValidType
            => new("FNZ0003", "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);
    }

    public static class Union
    {
        public static DiagnosticDescriptor TopLevelError
            => new("FNZ0004",
                "Class must be top level",
                "Class '{0}' using UnionGenerator must be top level or inside a partial class",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor WrongBaseType
            => new("FNZ0005", "Unions should not have a base class",
                "Class '{0}' should not have a base class",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor ObjectNotValidType
            => new("FNZ0006", "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor InterfaceNotValidType
            => new("FNZ0007", "User-defined conversions to or from an interface are not allowed",
                "User-defined conversions to or from an interface are not allowed",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor RepeatedTypeSymbols
            => new("FNZ0008", "Cannot use the same type twice in a union",
                "Cannot use the same type twice in a union",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true,
                "Type is used to manage unions. If you need to use the same type twice, use a wrapper type around what you need");
    }
}
