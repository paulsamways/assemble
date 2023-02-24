namespace Scheme.Tests.Expander;

public class ExpanderTests
{
    [Fact]
    public void LambdaExpandsToItself()
    {
        var o = new Parser(false).Parse("(lambda (x) x)");
        var stx = new Parser(true).Parse("(lambda (x) x)");

        var expander = new Scheme.Expander.Expander();
        var result = expander.Expand(stx);

        Assert.Equal(o, result);
    }

    [Fact]
    public void SimpleMacroTest()
    {
        var o = new Parser(true).Parse("(let-syntax ((one (lambda (stx) (quote-syntax (quote 1))))) (one))").To<SchemeSyntaxObject>();
        var expander = new Scheme.Expander.Expander();
        var e = expander.Expand(o);
        var vm = new Interpreter.VM();
        var result = vm.Run(e).To<SchemeNumber>();

        Assert.Equal(1, result.Value);
    }
}