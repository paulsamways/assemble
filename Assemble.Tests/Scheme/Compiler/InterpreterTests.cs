using Assemble.Scheme.Compiler.Instructions;
using Xunit.Abstractions;

namespace Assemble.Scheme.Compiler;

public class InterpreterTests
{
    private readonly ITestOutputHelper output;

    public InterpreterTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Constant_Test()
    {
        var interpreter = new Interpreter();
        var result = interpreter.Run((SchemeDatum)Parser.Parse("1")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Refer_Test()
    {
        var interpreter = new Interpreter();
        interpreter.Environment.Set(SchemeSymbol.FromString("a"), new SchemeNumber(2));
        var result = interpreter.Run((SchemeDatum)Parser.Parse("a")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void If_Test()
    {
        var interpreter = new Interpreter();
        var result = interpreter.Run((SchemeDatum)Parser.Parse("(if #t 1 2)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Lambda_Test()
    {
        var interpreter = new Interpreter();

        var result = interpreter.Run((SchemeDatum)Parser.Parse("((lambda (a) ((lambda (b) b) a)) 1)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void CallCC_NoEscape_Test()
    {
        var interpreter = new Interpreter();

        var result = interpreter.Run((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) 3)))")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public void CallCC_Escape_Test()
    {
        var interpreter = new Interpreter();

        var result = interpreter.Run((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) (* 100 (k 2)))))")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void LambdaMultipleExpression_Test()
    {
        var interpreter = new Interpreter();

        var result = interpreter.Run((SchemeDatum)Parser.Parse("((lambda (fib) (set! fib (lambda (n) (if (< n 2) n (+ (fib (- n 1)) (fib (- n 2)))))) (fib 15)) '())")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(610, result.Value);

    }

    [Fact]
    public void Set_Test()
    {
        var interpreter = new Interpreter();
        var result = interpreter.Run((SchemeDatum)Parser.Parse("((lambda (a) (set! a 2) a) 1)"));

        Assert.Equal(2, result.To<SchemeNumber>().Value);
    }

    [Fact]
    public void Builtin_Test()
    {
        var interpreter = new Interpreter();
        var result = interpreter.Run((SchemeDatum)Parser.Parse("(+ 1 1)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void TCO_Test()
    {
        var interpreter = new Interpreter();
        var maxdepth = 0;
        interpreter.Step += (object? sender, EventArgs e) => maxdepth = Math.Max(maxdepth, interpreter.StackFrame?.Depth ?? 0);

        var result = interpreter.Run((SchemeDatum)Parser.Parse("((lambda (ten) (set! ten (lambda (x) (if (< x 1000) (ten (+ x 1)) x))) (ten 0)) '())")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1000, result.Value);
        Assert.Equal(2, maxdepth);
    }
}