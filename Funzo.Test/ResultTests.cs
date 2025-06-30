using Funzo;
using System;
using System.Text.Json;

namespace Funzo.Test;
public class Result2ArityTests
{
    [Fact]
    public void Ok_Creates_A_Successful_Operation_Result()
    {
        var value = 7;
        var instance = Result<int, int>.Ok(7);
        var expected = OkOperation(value);
        var result = instance.Match(OkOperation, ErrOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Error_Creates_A_Failed_Operation_Result()
    {
        var value = 7;
        var instance = Result<int, int>.Err(7);
        var expected = ErrOperation(value);
        var result = instance.Match(OkOperation, ErrOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Match_When_Result_Is_Ok_Executes_Ok_Action()
    {
        var value = 13;
        var instance = Result<int, int>.Ok(value);

        instance.Match(Pass, Throw<int>("Match executed error function"));
    }

    [Fact]
    public void Match_When_Result_Is_Error_Executes_Error_Action()
    {
        var value = 13;
        var instance = Result<int, int>.Err(value);

        instance.Match(Throw<int>("Match executed ok function"), Pass);
    }

    [Fact]
    public void Map_Returns_New_Error_With_Ok_Value_Mapped()
    {
        var expected = 15;
        var initial = 0;
        var result = Result<int, int>.Ok(initial);

        var mappedResult = result.Map(_ => expected);

        mappedResult.Match(v => Assert.Equal(expected, v), Throw<int>("Match executed on error"));
    }

    [Fact]
    public void MapErr_Returns_New_Error_With_Err_Value_Mapped()
    {
        var expected = 15;
        var initial = 0;
        var result = Result<int, int>.Err(initial);

        var mappedResult = result.MapErr(_ => expected);

        mappedResult.Match(Throw<int>("Match executed on ok"), v => Assert.Equal(expected, v));
    }

    [Fact]
    public void IsErr_Returns_True_When_There_Is_An_Error()
    {
        var expectedText = "ERROR";
        var result = Result<string, string>.Err(expectedText);

        Assert.True(result.IsErr(out var errorText));

        Assert.Equal(expectedText, errorText);
    }

    [Fact]
    public void IsErr_Returns_False_When_There_Is_No_Error()
    {
        var expectedText = "OK";
        var result = Result<string, string>.Ok(expectedText);

        Assert.False(result.IsErr(out var ok, out var errorText));
        Assert.Null(errorText);
        Assert.Equal(expectedText, ok);
    }

    [Fact]
    public void InspectErr_Is_Not_Called_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("");

        var test = "TEST";

        result.InspectErr(_ => test = null);

        Assert.NotNull(test);
    }

    [Fact]
    public void Inspect_Is_Called_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        result.InspectErr(_ => test = null);

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_Async_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("TEST STRING");

        var test = "TEST";

        result.InspectErr(_ =>
        {
            test = null;
        });

        Assert.NotNull(test);
    }

    [Fact]
    public void Inspect_Is_Called_Async_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        result.InspectErr(_ =>
        {
            test = null;
        });

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Called_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("");

        var test = "TEST";

        result.Inspect(_ => test = null);

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        result.Inspect(_ => test = null);

        Assert.NotNull(test);
    }

    [Fact]
    public void Inspect_Is_Called_Async_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("TEST STRING");

        var test = "TEST";

        result.Inspect(_ =>
        {
            test = null;
        });

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_Async_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        result.Inspect(_ =>
        {
            test = null;
        });

        Assert.NotNull(test);
    }

    [Fact]
    public void Unwrap_Returns_Ok_Value_When_Ok()
    {
        var expected = 1;
        var result = Result<int, string>.Ok(expected);

        var returned = result.Unwrap();

        Assert.Equal(expected, returned);
    }

    [Fact]
    public void Unwrap_Throws_When_Err()
    {
        var result = Result<int, string>.Err("FAIL");

        Assert.Throws<ArgumentException>(() => result.Unwrap());
    }

    [Fact]
    public void AsOk_Returns_Option_With_Ok_Value()
    {
        var expected = 3;
        var result = Result<int, string>.Ok(expected);

        var option = result.AsOk();
        var isOk = option.IsSome(out var value);

        Assert.True(isOk);
        Assert.Equal(expected, value);
    }

    [Fact]
    public void AsOk_Returns_None_When_Err()
    {
        var result = Result<string, string>.Err("FAILURE");
        var option = result.AsOk();

        var isOk = option.IsSome(out var value);

        Assert.Null(value);
        Assert.False(isOk);
    }

    [Fact]
    public void Map_Flattens_Result()
    {
        var result = Result<int, string>.Ok(1);
        var mapped = result.Map(r => Result<string, string>.Ok(r.ToString()));

        var isErr = mapped.IsErr(out var ok, out _);

        Assert.False(isErr);
        Assert.Equal("1", ok);
    }

    [Fact]
    public void MapErr_Flattens_Result()
    {
        var result = Result<int, int>.Err(1);
        var mapped = result.MapErr(r => Result<int, string>.Err(r.ToString()));

        var isErr = mapped.IsErr( out var err);

        Assert.True(isErr);
        Assert.Equal("1", err);
    }
    private static int OkOperation(int value) => value + 1;
    private static int ErrOperation(int value) => value - 1;
    private static void Pass<T>(T _) { }
    private static Action<T> Throw<T>(string message) => (_) => throw new Exception(message);

}

public class Result1ArityTests
{
    [Fact]
    public void Result_Can_Be_Constructed_Only_With_Err_Parameter()
    {
        var okResult = Result<string>.Ok();
        var errResult = Result<string>.Err("FAILURE");

        Assert.False(okResult.IsErr(out _));
        Assert.True(errResult.IsErr(out _));
    }
}
