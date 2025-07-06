using Funzo.Serialization;
using System;
using System.Text.Json;

namespace Funzo.Test;
public class UnionSerializatorTests
{
    private readonly UnionConverterFactory _unionConverterFactory = new();

    [Fact]
    public void Serializes_Union_Type()
    {
        MyUnion stringUnion = "TEST";
        MyUnion intUnion = 33;
        MyUnion dateUnion = DateTime.Parse("2025-02-11");

        var converter = _unionConverterFactory.CreateConverter(typeof(MyUnion), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var strSerialized = JsonSerializer.Serialize(stringUnion, options);
        var intSerialized = JsonSerializer.Serialize(intUnion, options);
        var dateSerialized = JsonSerializer.Serialize(dateUnion, options);

        Console.WriteLine();
    }

    [Fact]
    public void Deserializes_Union_Type()
    {
        var strSerialized = @$"{{""String"":""TEST""}}";
        var intSerialized = @$"{{""Int32"":33}}";
        var dateSerialized = @$"{{""DateTime"":""2025-02-11T00:00:00Z""}}";

        var converter = _unionConverterFactory.CreateConverter(typeof(MyUnion), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var str = JsonSerializer.Deserialize<MyUnion>(strSerialized, options)!;
        var intg = JsonSerializer.Deserialize<MyUnion>(intSerialized, options)!;
        var date = JsonSerializer.Deserialize<MyUnion>(dateSerialized, options)!;

        var isString = str.Is<string>(out var s);
        Assert.True(isString);
        Assert.Equal("TEST", s);

        var isInt = intg.Is<int>(out var i);
        Assert.True(isInt);
        Assert.Equal(33, i);

        var isDate = date.Is<DateTime>(out var d);
        Assert.True(isDate);
        Assert.Equal(new DateTime(2025, 2,11,0,0,0,DateTimeKind.Utc), d);
    }
}

[Union]
public partial class MyUnion : Union<int, string, DateTime>;