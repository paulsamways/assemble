using Assemble.Interpreter.AST;

namespace Assemble.Tests;

public class InterpreterTests
{
    [Fact]
    public void Test_EvaluateNumber()
    {
        var i = new Interpreter.Interpreter();
        var result = i.Evaluate("42.0") as Number;

        Assert.NotNull(result);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Test_EvaluateString()
    {
        var i = new Interpreter.Interpreter();
        var result = i.Evaluate("\"Hello\"") as Interpreter.AST.String;

        Assert.NotNull(result);
        Assert.Equal("Hello", result.Value);
    }

    [Fact]
    public void Test_EvaluateQuoted()
    {
        var i = new Interpreter.Interpreter();
        var result = i.Evaluate("'(apple)") as List;

        Assert.NotNull(result);
        Assert.Equal("apple", ((Atom)result.Elements[0]).Name);
    }

    [Fact]
    public void Test_EvaluateFunction()
    {
        var i = new Interpreter.Interpreter();
        var result = i.Evaluate("(+ 1.0 2.0 3.0)") as Number;

        Assert.NotNull(result);
        Assert.Equal(6, result.Value);
    }

    [Fact]
    public void Test_EvaluateSetPrimitiveFunction()
    {
        var i = new Interpreter.Interpreter();
        var result = i.Evaluate("(set a 1.0)") as Number;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);

        Assert.True(i.Context.Has("a"));
    }
}