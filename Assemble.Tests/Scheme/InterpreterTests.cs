using Assemble.Scheme;

namespace Assemble.Tests.Scheme;

public class InterpreterTests
{
    private readonly Assemble.Scheme.Environment _environment;

    public InterpreterTests()
    {
        _environment = new();
    }

    [Fact]
    public void SchemeBoolean_SelfEvaluate()
    {
        Assert.Equal(SchemeBoolean.True, Parser.Parse("#t").Evaluate(_environment));
        Assert.Equal(SchemeBoolean.True, Parser.Parse("#f").Evaluate(_environment));
    }

    [Fact]
    public void SchemeNumber_SelfEvaluate()
    {
        Assert.Equal(new SchemeNumber(42), Parser.Parse("42").Evaluate(_environment));
    }
}