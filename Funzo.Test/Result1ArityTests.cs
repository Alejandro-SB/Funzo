using System;
using System.Threading.Tasks;

namespace Funzo.Test;
public class Result1ArityTests
{
    [Fact]
    public void Ok_Creates_A_Successful_Operation_Result()
    {
        var instance = Result<int>.Ok();
        instance.EnsureOk("Should not fail");
    }

    [Fact]
    public void Error_Creates_A_Failed_Operation_Result()
    {
        var instance = Result<int>.Err(7);
        Assert.Throws<ArgumentException>(() => instance.EnsureOk("Should fail"));
    }

    [Fact]
    public void Match_When_Result_Is_Ok_Executes_Ok_Action()
    {
        var instance = Result<int>.Ok();

        instance.Match(Pass, Throw<int>("Match executed error function"));
    }

    [Fact]
    public void Match_When_Result_Is_Error_Executes_Error_Action()
    {
        var value = 13;
        var instance = Result<int>.Err(value);

        instance.Match(Throw("Match executed ok function"), Pass);
    }

    [Fact]
    public void Map_Returns_New_Error_With_Ok_Value_Mapped()
    {
        var expected = 15;
        var result = Result<int>.Ok();

        var mappedResult = result.Map(() => expected);

        mappedResult.Match(v => Assert.Equal(expected, v), Throw<int>("Match executed on error"));
    }

    [Fact]
    public void MapErr_Returns_New_Error_With_Err_Value_Mapped()
    {
        var expected = 15;
        var result = Result<int>.Err(0);

        var mappedResult = result.MapErr(_ => expected);

        mappedResult.Match(Throw("Match executed on ok"), v => Assert.Equal(expected, v));
    }

    [Fact]
    public void IsErr_Returns_True_When_There_Is_An_Error()
    {
        var expectedText = "ERROR";
        var result = Result<string>.Err(expectedText);

        Assert.True(result.IsErr(out var errorText));

        Assert.Equal(expectedText, errorText);
    }

    [Fact]
    public void IsErr_Returns_False_When_There_Is_No_Error()
    {
        var result = Result<string>.Ok();

        Assert.False(result.IsErr(out var errorText));
        Assert.Null(errorText);
    }

    [Fact]
    public void InspectErr_Is_Not_Called_When_Result_Is_Ok()
    {
        var result = Result<string>.Ok();

        var test = "TEST";

        result.InspectErr(_ => test = null);

        Assert.NotNull(test);
    }

    [Fact]
    public void Inspect_Is_Called_When_Result_Is_Err()
    {
        var result = Result<string>.Err("");

        var test = "TEST";

        result.InspectErr(_ => test = null);

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_Async_When_Result_Is_Ok()
    {
        var result = Result<string>.Ok();

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
        var result = Result<string>.Err("");

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
        var result = Result<string>.Ok();

        var test = "TEST";

        result.Inspect(() => test = null);

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_When_Result_Is_Err()
    {
        var result = Result<string>.Err("");

        var test = "TEST";

        result.Inspect(() => test = null);

        Assert.NotNull(test);
    }

    [Fact]
    public void Inspect_Is_Called_Async_When_Result_Is_Ok()
    {
        var result = Result<string>.Ok();

        var test = "TEST";

        result.Inspect(() =>
        {
            test = null;
        });

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_Async_When_Result_Is_Err()
    {
        var result = Result<string>.Err("");

        var test = "TEST";

        result.Inspect(() =>
        {
            test = null;
        });

        Assert.NotNull(test);
    }

    [Fact]
    public void AsOk_Returns_Option_With_Ok_Value()
    {
        var result = Result<string>.Ok();

        var option = result.AsOk();
        var isOk = option.IsSome(out _);

        Assert.True(isOk);
    }

    [Fact]
    public void AsOk_Returns_None_When_Err()
    {
        var result = Result<string>.Err("FAILURE");
        var option = result.AsOk();

        var isOk = option.IsSome(out var value);

        Assert.Equal(Option<Unit>.None, value);
        Assert.False(isOk);
    }

    [Fact]
    public void Map_Flattens_Result()
    {
        var result = Result<string>.Ok();
        var mapped = result.Map(() => Result<string, string>.Ok("1"));

        var isErr = mapped.IsErr(out var ok, out _);

        Assert.False(isErr);
        Assert.Equal("1", ok);
    }

    [Fact]
    public void MapErr_Flattens_Result()
    {
        var result = Result<int>.Err(1);
        var mapped = result.MapErr(r => Result<string>.Err(r.ToString()));

        var isErr = mapped.IsErr(out var err);

        Assert.True(isErr);
        Assert.Equal("1", err);
    }

    [Fact]
    public async Task InspectAsync_Executes_Action_When_Ok()
    {
        var okResult = Result<string>.Ok();

        await Assert.ThrowsAsync<AccessViolationException>(() => okResult.InspectAsync(() => throw new AccessViolationException()));

        var errResult = Result<string>.Err("fail");

        await errResult.InspectAsync(() => throw new Exception("Should not reach"));
    }

    [Fact]
    public async Task InspectErrAsync_Executes_Action_When_Err()
    {
        var errResult = Result<string>.Err("fail");

        await Assert.ThrowsAsync<AccessViolationException>(() => errResult.InspectErrAsync(_ => throw new AccessViolationException()));

        var okResult = Result<string>.Ok();

        await okResult.InspectErrAsync(_ => throw new Exception("Should not reach"));
    }

    private static void Pass() { }
    private static void Pass<T>(T _) { }
    private static Action<T> Throw<T>(string message) => (_) => throw new Exception(message);
    private static Action Throw(string message) => () => throw new Exception(message);

}
