using Funzo.Serialization;
using System;
using System.Text.Json;

namespace Funzo.Test;
public class ResultSerializatorTests
{
    private readonly ResultConverterFactory _resultConverterFactory = new();

    [Fact]
    public void Result_Serializer_Serializes_Ok()
    {
        var num = 4;
        var result = Result<int, string>.Ok(num);
        var converter = _resultConverterFactory.CreateConverter(result.GetType(), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = JsonSerializer.Serialize(result, options);
        var expected = $@"{{""IsOk"":true,""Ok"":{num}}}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void Result_Serializer_Serializes_Err()
    {
        var text = "ERROR";
        var result = Result<int, string>.Err(text);
        var converter = _resultConverterFactory.CreateConverter(result.GetType(), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = JsonSerializer.Serialize(result, options);
        var expected = $@"{{""IsOk"":false,""Err"":""{text}""}}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Err_Result()
    {
        var text = "ERROR";
        var converter = _resultConverterFactory.CreateConverter(typeof(Result<int, string>), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = $@"{{""IsOk"":false,""Err"":""{text}""}}";
        var result = JsonSerializer.Deserialize<Result<int, string>>(serialized, options);

        Assert.NotNull(result);
        var isErr = result.IsErr(out var ok, out var err);

        Assert.True(isErr);
        Assert.Equal(text, err);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Ok_Result()
    {
        var number = 73;
        var text = $@"{{""IsOk"":true,""Ok"":{number}}}";
        var converter = _resultConverterFactory.CreateConverter(typeof(Result<int, string>), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var result = JsonSerializer.Deserialize<Result<int, string>>(text, options);

        Assert.NotNull(result);
        var isErr = result.IsErr(out var ok, out var err);

        Assert.False(isErr);
        Assert.Equal(number, ok);
    }

    [Fact]
    public void Result_Serializes_Deserializes_Custom_Result()
    {
        var text = @$"{{""IsOk"":true,""Ok"":1}}";

        var converter = _resultConverterFactory.CreateConverter(typeof(CustomResult), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var result = JsonSerializer.Deserialize<CustomResult>(text, options);

        Assert.NotNull(result);

        var isOk = !result.IsErr(out var ok, out _);

        Assert.True(isOk);
        Assert.Equal(1, ok);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Simple_Result()
    {
        var text = @$"{{""IsOk"":false,""Err"":""error""}}";

        var converter = _resultConverterFactory.CreateConverter(typeof(CustomSimpleResult), new JsonSerializerOptions())!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var result = JsonSerializer.Deserialize<CustomSimpleResult>(text, options);

        Assert.NotNull(result);

        var isErr = result.IsErr(out var oerr);

        Assert.True(isErr);
        Assert.Equal("error", oerr);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Simple_Result_Ok()
    {
        var text = @$"{{""IsOk"":true,""Ok"":""""}}";

        var simpleConverter = _resultConverterFactory.CreateConverter(typeof(CustomSimpleResult), new JsonSerializerOptions())!;
        var customConverter = _resultConverterFactory.CreateConverter(typeof(CustomResult), new JsonSerializerOptions())!;
        var options = new JsonSerializerOptions();
        options.Converters.Add(simpleConverter);
        options.Converters.Add(customConverter);

        var result = JsonSerializer.Deserialize<CustomSimpleResult>(text, options);

        Assert.NotNull(result);

        var isErr = result.IsErr(out var oerr);

        Assert.False(isErr);

        CustomResult crOk = 1;
        CustomResult crErr = "none";

        var csrOk = CustomSimpleResult.Ok();
        var csrErr = CustomSimpleResult.Err("FAIL");

        var serializedCustomOk = JsonSerializer.Serialize(crOk, options);
        var serializedCustomErr = JsonSerializer.Serialize(crErr, options);
        var serializedSimpleOk = JsonSerializer.Serialize(csrOk, options);
        var serializedSimpleErr = JsonSerializer.Serialize(csrErr, options);

        Assert.Equal($@"{{""IsOk"":true,""Ok"":1}}", serializedCustomOk);
        Assert.Equal($@"{{""IsOk"":false,""Err"":""none""}}", serializedCustomErr);
        Assert.Equal($@"{{""IsOk"":true,""Ok"":""""}}", serializedSimpleOk);
        Assert.Equal($@"{{""IsOk"":false,""Err"":""FAIL""}}", serializedSimpleErr);
    }
}

[Result<int, string>]
public partial class CustomResult;

[Result<string>]
public partial class CustomSimpleResult;