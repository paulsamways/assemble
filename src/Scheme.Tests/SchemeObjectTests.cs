using Scheme.Interpreter;

namespace Scheme.Tests;

public class SchemeObjectTests
{
    private readonly Parser _parser;

    private readonly SchemeObject _o;

    public SchemeObjectTests()
    {
        _parser = new Parser();
        _o = _parser.Parse("(a (b . #(5)))");
    }

    [Fact]
    public void Test_Visit_Replace()
    {
        var expected = _parser.Parse("(a (b 5))");

        var result = _o.Visit(new SchemeObjectVisitor()
        {
            OnSchemeVector = (x, xs) => new SchemePair(xs[0], SchemeEmptyList.Value)
        });

        Assert.Equal(expected, result);
    }
}