using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Funzo.Serialization;

public class UnionConverterFactory : JsonConverterFactory
{
    private static readonly Type[] ValidUnionTypes = [typeof(Union<,>), typeof(Union<,,>), typeof(Union<,,,>), typeof(Union<,,,,>)];

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && ValidUnionTypes.Contains(typeToConvert.GetGenericTypeDefinition());

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UnionConverter<TUnion> : JsonConverter<TUnion>
    where TUnion : UnionBase
{
    public override TUnion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, TUnion value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
