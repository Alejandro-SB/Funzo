using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funzo.Serialization;

/// <summary>
/// Factory to create a JsonConverter for any union derived class
/// </summary>
public class UnionConverterFactory : JsonConverterFactory
{
    private static readonly Type[] ValidUnionTypes = [
        typeof(Union<,>),
        typeof(Union<,,>),
        typeof(Union<,,,>),
        typeof(Union<,,,,>)
    ];

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.BaseType is { } baseType
        && baseType.IsGenericType
        && ValidUnionTypes.Contains(baseType.GetGenericTypeDefinition());

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(CanConvert(typeToConvert));

        var converter = (JsonConverter)Activator.CreateInstance(typeof(UnionJsonConverter<>).MakeGenericType([typeToConvert]))!;

        return converter;
    }
}

/// <summary>
/// JsonConverter for a union class of type <typeparamref name="TUnion"/>
/// </summary>
/// <typeparam name="TUnion">The type of the union</typeparam>
public class UnionJsonConverter<TUnion> : JsonConverter<TUnion>
    where TUnion : UnionBase
{
    /// <inheritdoc />
    public override TUnion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();

        if (reader.TokenType is not JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = reader.GetString() ?? throw new JsonException();

        if (typeToConvert.BaseType is not { } baseType)
        {
            throw new JsonException();
        }

        var types = baseType.GenericTypeArguments;
        var propertyType = types.FirstOrDefault(t => string.Equals(t.Name, propertyName, options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) ?? throw new JsonException();

        var value = JsonSerializer.Deserialize(ref reader, propertyType, options);
        reader.Read();

        if (reader.TokenType is not JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        var unionInstance = (TUnion)Activator.CreateInstance(typeof(TUnion), [value])!;

        return unionInstance;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TUnion value, JsonSerializerOptions options)
    {
        var innerValue = value.GetValue();
        var typeName = innerValue.GetType().Name;

        writer.WriteStartObject();
        writer.WritePropertyName(typeName);
        JsonSerializer.Serialize(writer, innerValue, options);
        writer.WriteEndObject();
    }
}
