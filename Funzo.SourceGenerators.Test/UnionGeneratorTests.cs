using System;

namespace Funzo.SourceGenerators.Test;
public class UnionGeneratorTests
{
    [Fact]
    public void Generates_Implicit_Conversors()
    {
        TestUnion u1 = "TEST";
        TestUnion u2 = 3;
        TestUnion u3 = DateTime.UtcNow;
        TestUnion u4 = DateTimeOffset.UtcNow;

        Assert.True(u1.Is<string>(out _));
        Assert.True(u2.Is<int>(out _));
        Assert.True(u3.Is<DateTime>(out _));
        Assert.True(u4.Is<DateTimeOffset>(out _));
    }

    [Fact]
    public void Generates_Constructors()
    {
        TestUnion u1 = new("TEST");
        TestUnion u2 = new(3);
        TestUnion u3 = new(DateTime.UtcNow);
        TestUnion u4 = new(DateTimeOffset.UtcNow);

        Assert.True(u1.Is<string>(out _));
        Assert.True(u2.Is<int>(out _));
        Assert.True(u3.Is<DateTime>(out _));
        Assert.True(u4.Is<DateTimeOffset>(out _));
    }

    [Fact]
    public void Generates_Shared_Property()
    {
        CommonPropertyUnion u1 = new A("a");
        CommonPropertyUnion u2 = new B("b");
        CommonPropertyUnion u3 = new C("c");

        Assert.Equal("a", u1.Text);
        Assert.Equal("b", u2.Text);
        Assert.Equal("c", u3.Text);
    }

    [Fact]
    public void Returns_Generic_Item_By_Interface()
    {
        var testString = "TEST";
        var first = GetUnion<TestUnion>(testString);
        var second = GetUnion<AnotherTestUnion>(testString);

        Assert.True(first.Is<string>(out var firstValue));
        Assert.True(second.Is<string>(out var secondValue));

        Assert.Equal(testString, firstValue);
        Assert.Equal(testString, secondValue);
    }

    private TOut GetUnion<TOut>(string value)
        where TOut: class, IUnion<string>
    {
        return TOut.From<TOut>(value);
    }
}

[Union<string, int, DateTime, DateTimeOffset>]
public partial class TestUnion;

[Union<string, short>]
public partial class AnotherTestUnion;

[Union<A, B, C>]
public partial class CommonPropertyUnion;

[Union<B, A>]
public partial record RecordUnion;

public class A(string text)
{
    public string Text { get; set; } = text;
    public int Number { get; set; }
}

public class B(string text)
{
    public string Text { get; set; } = text;
    protected int Number { get; set; }
}

public class C(string text)
{
    public string Text { get; set; } = text;
    public DateTime Date { get; set; }
}