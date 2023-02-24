using Scheme.Interpreter;

namespace Scheme.Tests;

public class SchemeSyntaxObjectTests
{
    private readonly Parser _syntaxObjectParser;
    private readonly Parser _parser;

    private readonly SchemeObject _syntaxObject;
    private readonly SchemeObject _object;

    public SchemeSyntaxObjectTests()
    {
        _parser = new Parser();
        _syntaxObjectParser = new Parser(true);

        _syntaxObject = _syntaxObjectParser.Parse("(a (b . #(5)))");
        _object = _parser.Parse("(a (b . #(5)))");
    }

    [Fact]
    public void Test_Visit_RemoveAllSyntaxObjects()
    {
        var result = _syntaxObject.Visit(new SchemeObjectVisitor()
        {
            OnSchemeSyntaxObject = (stx, datum) => datum
        });

        Assert.Equal(_object, result);
    }
}