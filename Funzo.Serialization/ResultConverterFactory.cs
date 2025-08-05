using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funzo.Serialization;

/// <summary>
/// Factory to create a JsonConverter for <see cref="Result{TOk, TErr}"/>
/// </summary>
public class ResultConverterFactory : JsonConverterFactory
{
    private static readonly Type[] ResultTypes = [typeof(ResultBase<,>), typeof(ResultBase<,,>)];

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.BaseType is Type baseType
            && baseType.IsGenericType
            && ResultTypes.Contains(baseType.GetGenericTypeDefinition());
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(CanConvert(typeToConvert));

        var baseType = typeToConvert.BaseType!;

        var genericTypes = baseType.GetGenericArguments();

        if (genericTypes.Length == 2)
        {
            var resultType = genericTypes[0];
            var errType = genericTypes[1];

            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(ResultConverter<,>)
            .MakeGenericType([resultType, errType]))!;

            return converter;
        }
        else
        {
            var resultType = genericTypes[0];
            var okType = genericTypes[1];
            var errType = genericTypes[2];

            var converter = (JsonConverter)Activator.CreateInstance(
                    typeof(ResultConverter<,,>)
                .MakeGenericType([resultType, okType, errType]))!;

            return converter;
        }
    }
}

/// <summary>
/// JsonConverter for <see cref="Result{TOk, TErr}"/>
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TErr"></typeparam>
public class ResultConverter<TResult, TOk, TErr> : JsonConverter<TResult>
    where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
{
    /// <inheritdoc />
    public override TResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var deserializedValue = JsonSerializer.Deserialize<ResultRepresentation<TOk, TErr>>(ref reader, options) ?? throw new JsonException();

        return deserializedValue.IsOk
            ? ResultConverter<TResult, TOk, TErr>.ProduceOk(deserializedValue.Ok!)
            : ResultConverter<TResult, TOk, TErr>.ProduceErr(deserializedValue.Err!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TResult result, JsonSerializerOptions options)
    {
        var isErr = result.IsErr(out var ok, out var err);

        object representation = isErr
            ? new ResultErrRepresentation<TErr>(err!)
            : new ResultOkRepresentation<TOk>(ok!);

        var content = JsonSerializer.Serialize(representation, options);

        writer.WriteRawValue(content);
    }

    private static TResult ProduceErr(TErr err)
    {
#if NET6_0_OR_GREATER
        return TResult.Err(err);
#else
        var staticErr = typeof(TResult).GetMethod("Err", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticErr.Invoke(null, [err!]);
#endif
    }

    private static TResult ProduceOk(TOk ok)
    {
#if NET6_0_OR_GREATER
        return TResult.Ok(ok);
#else
        var staticOk = typeof(TResult).GetMethod("Ok", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticOk.Invoke(null, [ok!]);
#endif
    }
}

/// <summary>
/// JsonConverter for <see cref="Result{TOk, TErr}"/>
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TErr"></typeparam>
public class ResultConverter<TResult, TErr> : JsonConverter<TResult>
    where TResult : ResultBase<TResult, TErr>, IResultBase<TResult, TErr>
{
    /// <inheritdoc />
    public override TResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var deserializedValue = JsonSerializer.Deserialize<ResultRepresentation<Unit, TErr>>(ref reader, options) ?? throw new JsonException();

        return deserializedValue.IsOk
            ? ResultConverter<TResult, TErr>.ProduceOk()
            : ResultConverter<TResult, TErr>.ProduceErr(deserializedValue.Err!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TResult result, JsonSerializerOptions options)
    {
        var isErr = result.IsErr(out var err);

        object representation = isErr
            ? new ResultErrRepresentation<TErr>(err!)
            : new ResultOkRepresentation<Unit>(default);

        var content = JsonSerializer.Serialize(representation, options);

        writer.WriteRawValue(content);
    }

    private static TResult ProduceErr(TErr err)
    {
#if NET6_0_OR_GREATER
        return TResult.Err(err);
#else
        var staticErr = typeof(TResult).GetMethod("Err", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticErr.Invoke(null, [err!]);
#endif
    }

    private static TResult ProduceOk()
    {
#if NET6_0_OR_GREATER
        return TResult.Ok();
#else
        var staticOk = typeof(TResult).GetMethod("Ok", BindingFlags.Static | BindingFlags.Public);
        return (TResult)staticOk.Invoke(null, []);
#endif
    }
}