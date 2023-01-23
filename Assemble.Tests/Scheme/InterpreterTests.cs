using Assemble.Scheme;

namespace Assemble.Tests.Scheme;

public class InterpreterTests
{
    private readonly Assemble.Scheme.Interpreter _interpreter;

    public InterpreterTests()
    {
        _interpreter = new();
    }

    [Fact]
    public void SchemeBoolean_SelfEvaluate()
    {
        Assert.Equal(SchemeBoolean.True, _interpreter.Evaluate("#t"));
        Assert.Equal(SchemeBoolean.True, _interpreter.Evaluate("#f"));
    }

    [Fact]
    public void SchemeNumber_SelfEvaluate()
    {
        Assert.Equal(new SchemeNumber(42), _interpreter.Evaluate("42"));
    }
}