using System;

namespace Funzo.SourceGenerators.Test;

public class ResultGeneratorTests
{
    [Fact]
    public void Generates_Implicit_Conversors_And_Constructors()
    {
        TestResult okImplicit = Unit.Default;
        TestResult errImplicit = "FAILURE";

        Assert.False(okImplicit.IsErr(out _));
        Assert.True(errImplicit.IsErr(out _));
    }

    [Fact]
    public void Generates_Ok_And_Err_Methods()
    {
        var ok = TestResult.Ok(Unit.Default);
        var err = TestResult.Err("FAIL");

        Assert.False(ok.IsErr(out _));
        Assert.True(err.IsErr(out _));
    }

    [Fact]
    public void Generates_Result_With_Just_Err_Type()
    {
        var ok = TestUnitResult.Ok();
        var fail = TestUnitResult.Err("FAILURE");

        Assert.False(ok.IsErr(out _));
        Assert.True(fail.IsErr(out _));
    }

    [Fact]
    public void Can_Implicitly_Convert_From_Union_In_Generic_To_Type()
    {
        static TwoUnionResult process(int value)
        {
            return value switch
            {
                1 => new ArgumentError(),
                2 => new InvalidError(),
                3 => new UnknownError(),
                4 => 4,
                5 => "STRING"
            };
        }

        var err = process(1);

        var isErr = err.IsErr(out var e);

        Assert.True(isErr);

        e!.Switch(a => { }, b => throw new Exception("Should not reach"), c => throw new Exception("Should not reach"));

        var ok = process(4);

        var isOk = !ok.IsErr(out var o, out _);

        Assert.True(isOk);
    }
}

[Result]
public partial class TestResult : IResult<Unit, string>;
[Result]
public partial class TestUnitResult : IResult<string>;

[Union]
public partial class MyOk : Union<int, string>;

[Result]
public partial class TwoUnionResult : IResult<MyOk, MyError>;

[Union]
public partial class ClonedErr : Union<string, int>;

[Union]
public partial class ClonedOk : Union<string, int>;

[Result]
public partial class CheapClone : IResult<ClonedOk, ClonedErr>;