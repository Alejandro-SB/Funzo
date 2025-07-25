﻿namespace Funzo.SourceGenerators;
internal static class FunzoAttributeSources
{
    internal const string ResultAttributeName = "ResultAttribute";
    internal const string UnionAttributeName = "UnionAttribute";
    internal const string AttributeNamespace = "Funzo";
    internal static readonly string[] ResultAttributeFullNames = [
        $"{AttributeNamespace}.{ResultAttributeName}<>",
        $"{AttributeNamespace}.{ResultAttributeName}<,>"
    ];
    internal static readonly string[] UnionAttributeFullNames = [
        $"{AttributeNamespace}.{UnionAttributeName}<,>",
        $"{AttributeNamespace}.{UnionAttributeName}<,,>",
        $"{AttributeNamespace}.{UnionAttributeName}<,,,>",
        $"{AttributeNamespace}.{UnionAttributeName}<,,,,>"
    ];
    internal const string ResultAttributeContent = $@"// <auto-generated />
using System;

#pragma warning disable 1591

namespace {AttributeNamespace}
{{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {ResultAttributeName}<TErr> : Attribute
    {{
    }}

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {ResultAttributeName}<TOk, TErr> : Attribute
    {{
    }}
}}
";

    internal const string UnionAttributeContent = $@"// <auto-generated />
using System;

#pragma warning disable 1591

namespace {AttributeNamespace}
{{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {UnionAttributeName}<T0, T1> : Attribute
    {{
    }}

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {UnionAttributeName}<T0, T1, T2> : Attribute
    {{
    }}

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {UnionAttributeName}<T0, T1, T2, T3> : Attribute
    {{
    }}

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class {UnionAttributeName}<T0, T1, T2, T3, T4> : Attribute
    {{
    }}
}}
";
}
