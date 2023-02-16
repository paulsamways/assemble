using Scheme.Compiler;
using Xunit.Abstractions;

namespace Scheme.Tests.Compiler;

public class VMTests
{
    private readonly ITestOutputHelper output;

    public VMTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Constant_Test()
    {
        var vm = new VM();
        var result = vm.Run((SchemeDatum)Parser.Parse("1")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Refer_Test()
    {
        var vm = new VM();
        vm.Environment.Set(SchemeSymbol.FromString("a"), new SchemeNumber(2));
        var result = vm.Run((SchemeDatum)Parser.Parse("a")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void If_Test()
    {
        var vm = new VM();
        var result = vm.Run((SchemeDatum)Parser.Parse("(if #t 1 2)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Lambda_Test()
    {
        var vm = new VM();

        var result = vm.Run((SchemeDatum)Parser.Parse("((lambda (a) ((lambda (b) b) a)) 1)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void CallCC_NoEscape_Test()
    {
        var vm = new VM();

        var result = vm.Run((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) 3)))")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public void CallCC_Escape_Test()
    {
        var vm = new VM();

        var result = vm.Run((SchemeDatum)Parser.Parse("(+ 1 (call/cc (lambda (k) (* 100 (k 2)))))")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void LambdaMultipleExpression_Test()
    {
        var vm = new VM();

        var result = vm.Run((SchemeDatum)Parser.Parse("((lambda (fib) (set! fib (lambda (n) (if (< n 2) n (+ (fib (- n 1)) (fib (- n 2)))))) (fib 15)) '())")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(610, result.Value);

    }

    [Fact]
    public void Set_Test()
    {
        var vm = new VM();
        var result = vm.Run((SchemeDatum)Parser.Parse("((lambda (a) (set! a 2) a) 1)"));

        Assert.Equal(2, result.To<SchemeNumber>().Value);
    }

    [Fact]
    public void Builtin_Test()
    {
        var vm = new VM();
        var result = vm.Run((SchemeDatum)Parser.Parse("(+ 1 1)")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void TailCallOptimisation_Test()
    {
        var vm = new VM();
        var maxdepth = 0;
        vm.Step += (object? sender, EventArgs e) => maxdepth = Math.Max(maxdepth, vm.StackFrame?.Depth ?? 0);

        var result = vm.Run((SchemeDatum)Parser.Parse("((lambda (ten) (set! ten (lambda (x) (if (< x 1000) (ten (+ x 1)) x))) (ten 0)) '())")) as SchemeNumber;

        Assert.NotNull(result);
        Assert.Equal(1000, result.Value);
        Assert.Equal(2, maxdepth);
    }

    [Fact]
    public void RunProcedure()
    {
        var vm = new VM();
        var proc = vm.Run((SchemeDatum)Parser.Parse("(lambda (x) x)"));

        Assert.IsType<SchemeProcedure>(proc);

        var result = vm.Run((SchemeProcedure)proc, new SchemeNumber(5));

        Assert.IsType<SchemeNumber>(result);
        Assert.Equal(5, result.To<SchemeNumber>().Value);
    }
}