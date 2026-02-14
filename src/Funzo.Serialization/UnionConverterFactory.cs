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
        if (typeToConvert.BaseType is not { } baseType)
        {
            throw new JsonException("Union should inherit from UnionBase");
        }

        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();

        if (reader.TokenType is not JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var taggedType = GetTaggedType(ref reader, baseType);

        if (reader.TokenType is not JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = reader.GetString() ?? throw new JsonException();

        if (!string.Equals(propertyName, nameof(UnionSerializedRepresentation.Value), StringComparison.OrdinalIgnoreCase))
        {
            throw new JsonException($"Unable to find the {nameof(UnionSerializedRepresentation.Value)} property inside the union");
        }

        reader.Read();

        var value = JsonSerializer.Deserialize(ref reader, taggedType, options);

        // Completely consume the reader
        do
        {
            reader.Read();
        } while (reader.TokenType != JsonTokenType.EndObject);

        var unionInstance = (TUnion)Activator.CreateInstance(typeof(TUnion), [value])!;

        return unionInstance;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TUnion value, JsonSerializerOptions options)
    {
        var innerValue = value.GetValue();

        var representation = new UnionSerializedRepresentation
        {
            Value = innerValue
        };

        JsonSerializer.Serialize(writer, representation, options);
    }

    /// <summary>
    /// Checks for the tag property and reads its value, returning the matching type in the union
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private static Type GetTaggedType(ref Utf8JsonReader reader, Type baseType)
    {
        var propertyName = reader.GetString() ?? throw new JsonException();
        string? typeName;

        if (string.Equals(propertyName, nameof(UnionSerializedRepresentation.Tag), StringComparison.OrdinalIgnoreCase))
        {
            reader.Read();
            typeName = reader.GetString();
            reader.Read();
        }
        else
        {
            // Clone the reader to preserve current position, as we now want to read the value property
            var clonedReader = reader;
            // Skip union content
            clonedReader.Read();
            // Skip tag property
            clonedReader.Read();
            // Read tag property content
            clonedReader.Read();

            typeName = clonedReader.GetString();
        }

        if(typeName is null)
        {
            throw new JsonException("Unable to find the tag property in the union");
        }

        return GetTypeFromTag(baseType, typeName);
    }

    private static Type GetTypeFromTag(Type baseType, string typeName)
    {
        var types = baseType.GenericTypeArguments;
        var unionType = types.FirstOrDefault(t => t.Name == typeName) ?? throw new JsonException();

        return unionType;
    }
}
