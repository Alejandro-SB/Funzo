using Microsoft.CodeAnalysis;

namespace Funzo.SourceGenerators.Helpers;

internal static class FunzoDiagnosticDescriptors
{
    public static class Result
    {
        public static DiagnosticDescriptor TopLevelError
            => GenerateResultDescriptor("FNZ0001",
                "Class must be top level",
                "Class '{0}' using ResultGenerator must be top level or inside a partial class");

        public static DiagnosticDescriptor WrongBaseType
            => GenerateResultDescriptor("FNZ0002",
                "Result should not have a base class",
                "Class '{0}' should not have any base class");

        public static DiagnosticDescriptor ObjectNotValidType
            => GenerateResultDescriptor("FNZ0003",
                "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'");

        public static DiagnosticDescriptor RepeatedTypeSymbols
            => GenerateResultDescriptor("FNZ0004",
                "Types in Result<TOk, TErr> cannot be the same",
                "The types in a source generated result cannot be the same"
                );

        private static DiagnosticDescriptor GenerateResultDescriptor(string id, string title, string message, string? description = null)
            => GenerateDescriptor(id, title, message, "ResultGenerator", description);
    }

    public static class Union
    {
        public static DiagnosticDescriptor TopLevelError
            => GenerateUnionDescriptor("FNZ1000",
                "Class must be top level",
                "Class '{0}' using UnionGenerator must be top level or inside a partial class");

        public static DiagnosticDescriptor WrongBaseType
            => GenerateUnionDescriptor("FNZ1001",
                "Unions should not have a base class",
                "Class '{0}' should not have a base class");

        public static DiagnosticDescriptor ObjectNotValidType
            => GenerateUnionDescriptor("FNZ1002",
                "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'");

        public static DiagnosticDescriptor InterfaceNotValidType
            => GenerateUnionDescriptor("FNZ1003",
                "User-defined conversions to or from an interface are not allowed",
                "User-defined conversions to or from an interface are not allowed");

        public static DiagnosticDescriptor RepeatedTypeSymbols
            => GenerateUnionDescriptor("FNZ1004",
                "Cannot use the same type twice in a union",
                "Cannot use the same type twice in a union",
                "Type is used to manage unions. If you need to use the same type twice, use a wrapper type around what you need");

        private static DiagnosticDescriptor GenerateUnionDescriptor(string id, string title, string message, string? description = null)
            => GenerateDescriptor(id, title, message, "UnionGenerator", description);
    }

    private static DiagnosticDescriptor GenerateDescriptor(string id, string title, string message, string category, string? description = null)
        => new(id, title, message, category, DiagnosticSeverity.Error, true, description);
}
