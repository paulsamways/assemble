using Scheme.Compiler;

namespace Scheme.Expander.Tests;

public class ExpanderTests
{
    [Fact]
    public void LambdaExpandsToItself()
    {
        var o = new Parser(true).Parse("(lambda (x) x)").To<SchemeSyntaxObject>();

        var expander = new Expander();
        var result = expander.Expand(o);

        Assert.Equal(o.ToDatum(), result.ToDatum());
    }

    [Fact]
    public void SimpleMacroTest()
    {
        var o = new Parser(true).Parse("(let-syntax ((one (lambda (stx) (quote-syntax '1)))) (one))").To<SchemeSyntaxObject>();
        var expander = new Expander();
        var e = expander.Expand(o);

        Assert.IsType<SchemeNumber>(e);
    }
}