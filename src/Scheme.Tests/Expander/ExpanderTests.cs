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
}