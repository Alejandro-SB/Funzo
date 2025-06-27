namespace Funzo.SourceGenerators.Test;
public class ResultBuilderTEsts
{
    [Fact]
    public void ResultBuilder_Returns_Correct_Error_Based_On_Exception_Thrown()
    {
        var builder = MyResult.For()
            .MapException<ArgumentNullException>(ane => new ArgumentError())
            .MapException<InvalidOperationException>(ioe => new InvalidError())
            .MapElse(e => new UnknownError());

        var argumentResult = builder.Try(() => throw new ArgumentNullException());
        var invalidResult = builder.Try(() => throw new InvalidOperationException());
        var anyOtherResult = builder.Try(() => throw new AbandonedMutexException());

        var isArgumentErr = argumentResult.IsErr(out var argumentError);
        var isInvalidErr = invalidResult.IsErr(out var invalidError);
        var isAnyOtherErr = anyOtherResult.IsErr(out var otherError);

        Assert.True(isArgumentErr);
        Assert.True(isInvalidErr);
        Assert.True(isAnyOtherErr);

        argumentError!.Switch(x => Assert.IsType<ArgumentError>(x), Throw, Throw);
        invalidError!.Switch(Throw, x => Assert.IsType<InvalidError>(x), Throw);
        otherError!.Switch(Throw, Throw, x => Assert.IsType<UnknownError>(x));
    }

    [Fact]
    public void ResultBuilder_Throws_Exception_If_It_Is_Not_Captured()
    {
        var builder = MyResult.For()
            .MapException<ArgumentNullException>(ane => new ArgumentError())
            .MapException<InvalidOperationException>(ioe => new InvalidError());

        Assert.Throws<InsufficientMemoryException>(() => builder.Try(() => throw new InsufficientMemoryException()));
    }

    [Fact]
    public void ResultBuilder_Returns_Ok_Result_When_No_Errors()
    {
        var expected = "OK";
        var builder = MyCustomResult.For()
            .MapException<ArgumentNullException>(ane => new ArgumentError())
            .MapException<InvalidOperationException>(ioe => new InvalidError())
            .MapElse(e => new UnknownError());

        var result = builder.Try(() => expected);

        var isErr = result.IsErr(out var ok, out var err);

        Assert.False(isErr);
        Assert.Equal(expected, ok);
        Assert.Null(err);
    }

    private static void Throw<T>(T any) => throw new Exception("Should not get here");
}


[Result]
public partial class MyResult : Result<MyError>;

[Result]
public partial class MyCustomResult : Result<string, MyError>;

[Union]
public partial class MyError : Union<ArgumentError, InvalidError, UnknownError>;

public class ArgumentError;
public class InvalidError;
public class UnknownError;