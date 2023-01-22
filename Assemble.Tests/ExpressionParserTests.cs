using System.Security.Cryptography;
using System.Text;
using Assemble.Interpreter;

namespace Assemble.Tests;

public class ExpressionParserTests
{
    [Fact]
    public void Test_ParseString()
    {
        const string input = "\"Hello World\"";
        var e = ExpressionParser.Parse(input) as Interpreter.AST.String;

        Assert.NotNull(e);
        Assert.Equal("Hello World", e.Value);
    }

    [Fact]
    public void Test_ParseAtom()
    {
        const string input = "hello";
        var e = ExpressionParser.Parse(input) as Interpreter.AST.Atom;

        Assert.NotNull(e);
        Assert.Equal("hello", e.Name);
    }

    [Fact]
    public void Test_ParseNumber()
    {
        const string input = "42.0";
        var e = ExpressionParser.Parse(input) as Interpreter.AST.Number;

        Assert.NotNull(e);
        Assert.Equal(42, e.Value);
    }

    [Fact]
    public void Test_ParseList()
    {
        const string input = "(1.0 2.0 3.0)";
        var e = ExpressionParser.Parse(input) as Interpreter.AST.List;

        Assert.NotNull(e);
        Assert.Equal(3, e.Elements.Length);
        Assert.Equal(1, ((Interpreter.AST.Number)e.Elements[0]).Value);
        Assert.Equal(2, ((Interpreter.AST.Number)e.Elements[1]).Value);
        Assert.Equal(3, ((Interpreter.AST.Number)e.Elements[2]).Value);
    }

    [Fact]
    public void Test_ParseQuotedList()
    {
        const string input = "'(hello 1.0 2.0)";

        var e = ExpressionParser.Parse(input) as Interpreter.AST.List;

        Assert.NotNull(e);
        Assert.Equal("quote", ((Interpreter.AST.Atom)e.Elements[0]).Name);

        var xs = e.Elements[1] as Interpreter.AST.List;
        Assert.NotNull(xs);

        Assert.Equal("hello", ((Interpreter.AST.Atom)xs.Elements[0]).Name);
        Assert.Equal(1, ((Interpreter.AST.Number)xs.Elements[1]).Value);
        Assert.Equal(2, ((Interpreter.AST.Number)xs.Elements[2]).Value);
    }
}