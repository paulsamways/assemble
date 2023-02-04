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
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("1"));
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Refer_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("a"));
        interpreter.Environment.Set(SchemeSymbol.FromString("a"), new SchemeNumber(2));
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void If_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("(if #t 1 2)"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Lambda_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("((lambda (a) a) 1)"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void CallCC_NoEscape_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) 3)))"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public void CallCC_Escape_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) (* 100 (k 2)))))"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void LambdaMultipleExpression_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("((lambda (fib) (set! fib (lambda (n) (if (< n 2) n (+ (fib (- n 1)) (fib (- n 2)))))) (fib 5)) '())"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void Set_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("((lambda (a) (set! a 2) a) 1)"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run();

        Assert.Equal(2, result.To<SchemeNumber>().Value);
    }

    [Fact]
    public void Builtin_Test()
    {
        var interpreter = new Interpreter((SchemeDatum)Parser.Parse("(null? '())"));
        output.WriteLine(interpreter.Instructions.ToString());
        var result = interpreter.Run() as SchemeBoolean;

        Assert.NotNull(result);
        Assert.True(result.Value);
    }
}